using System;
using System.Globalization;
using System.Text.RegularExpressions;
using NutriAR.Models;
using UnityEngine;
using UnityEngine.Networking;

namespace NutriAR.Services
{
    public sealed class OpenFoodFactsClient : MonoBehaviour
    {
        [Serializable]
        private sealed class ProductResponse
        {
            public ProductData product;
            public ApiResult result;
        }

        [Serializable]
        private sealed class ApiResult
        {
            public string id;
            public string name;
        }

        [Serializable]
        private sealed class ProductData
        {
            public string product_name;
            public string product_name_pt;
            public string product_name_pt_br;
            public string product_name_en;
            public string brands;
            public string quantity;
            public string serving_size;
            public string categories;
            public Nutriments nutriments;
        }

        [Serializable]
        private sealed class Nutriments
        {
            public float carbohydrates_100g;
            public float carbohydrates;
            public float proteins_100g;
            public float proteins;
            public float fat_100g;
            public float fat;
            public float fiber_100g;
            public float fiber;
            public float sugars_100g;
            public float sugars;
            public float sodium_100g;
            public float sodium;
            public float saturated_fat_100g;
            public float saturated_fat;
        }

        public void Lookup(string barcode, Action<NutritionProfile, string> completed)
        {
            StartCoroutine(LookupRoutine(barcode, completed));
        }

        private System.Collections.IEnumerator LookupRoutine(string barcode, Action<NutritionProfile, string> completed)
        {
            if (string.IsNullOrWhiteSpace(barcode) || !Regex.IsMatch(barcode, "^[0-9]{8,14}$"))
            {
                completed?.Invoke(null, "Código de barras inválido. Tente novamente.");
                yield break;
            }

            var url = "https://world.openfoodfacts.org/api/v3/product/" + barcode +
                      "?product_type=food&lc=pt&fields=product_name,product_name_pt,product_name_pt_br,product_name_en,brands,quantity,serving_size,categories,nutriments";
            using (var request = UnityWebRequest.Get(url))
            {
                request.timeout = 12;
                request.SetRequestHeader("User-Agent", "NutriAR/0.2 (https://github.com/maiaJhfB/Aplicativo-Orienta-Nutricional)");
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    completed?.Invoke(null, "Não foi possível consultar o produto. Verifique a internet e tente novamente.");
                    yield break;
                }

                ProductResponse response;
                try
                {
                    response = JsonUtility.FromJson<ProductResponse>(request.downloadHandler.text);
                }
                catch (Exception)
                {
                    completed?.Invoke(null, "A base de produtos retornou dados que não conseguimos ler.");
                    yield break;
                }

                if (response == null || response.product == null || response.product.nutriments == null)
                {
                    completed?.Invoke(null, "Produto não encontrado com dados nutricionais. Você pode escolher um exemplo ou informar os dados manualmente.");
                    yield break;
                }

                var profile = CreateProfile(response.product, request.downloadHandler.text, barcode);
                if (profile == null)
                {
                    completed?.Invoke(null, "O produto foi encontrado, mas não há informações de calorias suficientes.");
                    yield break;
                }

                completed?.Invoke(profile, null);
            }
        }

        private static NutritionProfile CreateProfile(ProductData product, string rawJson, string barcode)
        {
            var nutrients = product.nutriments;
            var servingGrams = ParseServingGrams(product.serving_size);
            // Only treat explicitly normalized values as per 100 g. Open Food Facts also
            // exposes a generic "value" and a per-serving value; multiplying either by
            // servingGrams/100 can silently distort the displayed calories.
            var caloriesPer100g = ReadRawNutrient(rawJson, "energy-kcal_100g");
            if (caloriesPer100g <= 0f)
            {
                caloriesPer100g = ReadRawNutrient(rawJson, "energy_100g");
                if (caloriesPer100g > 0f)
                {
                    caloriesPer100g /= 4.184f;
                }
            }

            var carbs = ReadPer100g(nutrients.carbohydrates_100g, nutrients.carbohydrates);
            var proteins = ReadPer100g(nutrients.proteins_100g, nutrients.proteins);
            var fats = ReadPer100g(nutrients.fat_100g, nutrients.fat);
            var caloriesPerServing = 0f;
            if (caloriesPer100g <= 0f)
            {
                caloriesPerServing = ReadRawNutrient(rawJson, "energy-kcal_serving");
            }

            if (caloriesPer100g <= 0f && caloriesPerServing <= 0f && carbs + proteins + fats > 0f)
            {
                caloriesPer100g = carbs * 4f + proteins * 4f + fats * 9f;
            }

            if (caloriesPer100g <= 0f && caloriesPerServing <= 0f)
            {
                return null;
            }

            var factor = servingGrams / 100f;
            var name = FirstText(product.product_name_pt_br, product.product_name_pt, product.product_name, product.product_name_en);
            if (string.IsNullOrEmpty(name)) name = "Produto " + barcode;
            if (!string.IsNullOrWhiteSpace(product.brands)) name += " • " + product.brands;
            var portion = string.IsNullOrWhiteSpace(product.serving_size) ? (servingGrams.ToString("0.#", CultureInfo.GetCultureInfo("pt-BR")) + " g (estimado)") : product.serving_size;

            var addedSugar = ReadRawNutrient(rawJson, "added-sugars_100g", "added-sugars");
            var sodiumPer100g = ReadPer100g(nutrients.sodium_100g, nutrients.sodium) * 1000f;
            var saturatedFatPer100g = ReadRawNutrient(rawJson, "saturated-fat_100g", "saturated-fat_value");
            if (saturatedFatPer100g <= 0f)
            {
                saturatedFatPer100g = ReadPer100g(nutrients.saturated_fat_100g, nutrients.saturated_fat);
            }
            var sugar = ReadPer100g(nutrients.sugars_100g, nutrients.sugars) * factor;
            var sodium = sodiumPer100g * factor;
            var saturatedFat = saturatedFatPer100g * factor;
            var isLiquid = (!string.IsNullOrEmpty(product.quantity) && product.quantity.IndexOf("ml", StringComparison.OrdinalIgnoreCase) >= 0) ||
                           (!string.IsNullOrEmpty(product.categories) && (product.categories.IndexOf("beverage", StringComparison.OrdinalIgnoreCase) >= 0 || product.categories.IndexOf("drink", StringComparison.OrdinalIgnoreCase) >= 0));
            var sugarLimit = isLiquid ? 7.5f : 15f;
            var saturatedFatLimit = isLiquid ? 3f : 6f;
            var sodiumLimit = isLiquid ? 300f : 600f;
            var alertTitle = "Dados nutricionais encontrados";
            var alertMessage = "Valores informados por usuários no Open Food Facts; podem estar incompletos ou desatualizados. Confira o rótulo.";
            var guidance = "A porção pode variar. Compare a quantidade consumida com o tamanho indicado no rótulo.";
            var alertLevel = HealthAlertLevel.Attention;

            if (addedSugar >= sugarLimit || saturatedFatPer100g >= saturatedFatLimit || sodiumPer100g >= sodiumLimit)
            {
                alertTitle = "Alto teor em pelo menos um nutriente";
                var basis = isLiquid ? "100 ml: " : "100 g: ";
                alertMessage = basis + (addedSugar >= sugarLimit ? "açúcares adicionados ≥ " + sugarLimit + " g; " : "") +
                               (saturatedFatPer100g >= saturatedFatLimit ? "gordura saturada ≥ " + saturatedFatLimit + " g; " : "") +
                               (sodiumPer100g >= sodiumLimit ? "sódio ≥ " + sodiumLimit + " mg; " : "") +
                               "limites de referência da rotulagem frontal da Anvisa. Açúcares totais nesta porção: " + sugar.ToString("0.#", CultureInfo.GetCultureInfo("pt-BR")) + " g.";
                alertLevel = HealthAlertLevel.High;
            }
            else if (sugar >= 15f)
            {
                alertTitle = "Observe os açúcares totais";
                alertMessage = "Esta porção tem " + sugar.ToString("0.#", CultureInfo.GetCultureInfo("pt-BR")) + " g de açúcares totais. Esse dado não separa açúcares naturais e adicionados.";
            }
            else if (proteins >= 8f || ReadPer100g(nutrients.fiber_100g, nutrients.fiber) * factor >= 4f)
            {
                alertTitle = "Há nutrientes que ajudam na saciedade";
                alertMessage = "Confira proteínas e fibras no contexto da porção e do restante da refeição.";
                alertLevel = HealthAlertLevel.Positive;
            }

            return new NutritionProfile(
                barcode,
                name,
                portion,
                Mathf.RoundToInt(caloriesPerServing > 0f ? caloriesPerServing : caloriesPer100g * factor),
                carbs * factor,
                proteins * factor,
                fats * factor,
                ReadPer100g(nutrients.fiber_100g, nutrients.fiber) * factor,
                sugar,
                sodium,
                alertLevel,
                alertTitle,
                alertMessage,
                guidance,
                saturatedFat,
                servingGrams,
                barcode,
                true);
        }

        private static float ParseServingGrams(string serving)
        {
            if (!string.IsNullOrWhiteSpace(serving))
            {
                var match = Regex.Match(serving, @"([0-9]+(?:[\.,][0-9]+)?)\s*(?:g|ml)\b", RegexOptions.IgnoreCase);
                if (match.Success && float.TryParse(match.Groups[1].Value.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out var grams) && grams > 0f)
                {
                    return grams;
                }
            }
            return 100f;
        }

        private static float ReadRawNutrient(string json, params string[] keys)
        {
            foreach (var key in keys)
            {
                var match = Regex.Match(json, "\\\"" + Regex.Escape(key) + "\\\"\\s*:\\s*(-?[0-9]+(?:\\.[0-9]+)?)", RegexOptions.IgnoreCase);
                if (match.Success && float.TryParse(match.Groups[1].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
                {
                    return value;
                }
            }
            return 0f;
        }

        private static float ReadPer100g(float valuePer100g, float fallback)
        {
            return valuePer100g > 0f ? valuePer100g : Mathf.Max(0f, fallback);
        }

        private static string FirstText(params string[] values)
        {
            for (var i = 0; i < values.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(values[i])) return values[i].Trim();
            }
            return null;
        }
    }
}

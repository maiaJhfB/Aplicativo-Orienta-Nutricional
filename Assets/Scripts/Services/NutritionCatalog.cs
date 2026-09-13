using System.Collections.Generic;
using NutriAR.Models;

namespace NutriAR.Services
{
    public static class NutritionCatalog
    {
        private static readonly NutritionProfile[] Items =
        {
            new NutritionProfile(
                "banana",
                "Banana-prata",
                "1 unidade média (118 g)",
                105,
                27f,
                1.3f,
                0.4f,
                3.1f,
                14.4f,
                1f,
                HealthAlertLevel.Positive,
                "Boa fonte de energia rápida",
                "Tem fibras e potássio. O açúcar está naturalmente presente na fruta.",
                "Combine com iogurte natural ou castanhas para acrescentar proteínas e gorduras boas.") ,
            new NutritionProfile(
                "refrigerante",
                "Refrigerante cola",
                "1 lata (350 ml)",
                149,
                37f,
                0f,
                0f,
                0f,
                37f,
                18f,
                HealthAlertLevel.High,
                "Alto teor de açúcar",
                "Esta porção contém cerca de 9 colheres de chá de açúcar e quase não oferece fibras.",
                "Se puder, reduza a frequência ou alterne com água, água com gás ou bebida sem açúcar.") ,
            new NutritionProfile(
                "barra",
                "Barra de cereal",
                "1 unidade (25 g)",
                92,
                17f,
                1.6f,
                2.2f,
                1.5f,
                7.5f,
                67f,
                HealthAlertLevel.Attention,
                "Observe a lista de ingredientes",
                "Barras parecidas podem ter quantidades bem diferentes de açúcar e fibras.",
                "Compare rótulos e prefira as opções com mais fibras e menos açúcares adicionados.") ,
            new NutritionProfile(
                "pao-queijo",
                "Pão de queijo",
                "1 unidade grande (50 g)",
                180,
                22f,
                3.8f,
                8.2f,
                0.6f,
                0.8f,
                320f,
                HealthAlertLevel.Attention,
                "Atenção ao sódio e à porção",
                "Duas unidades dobram a estimativa para 360 kcal e 640 mg de sódio.",
                "Acompanhe com fruta e uma fonte de proteína para uma refeição mais completa.") ,
            new NutritionProfile(
                "iogurte",
                "Iogurte natural",
                "1 pote (170 g)",
                114,
                8.5f,
                6.0f,
                6.1f,
                0f,
                8.5f,
                82f,
                HealthAlertLevel.Positive,
                "Opção com proteínas",
                "O iogurte natural fornece proteína e cálcio, com pouco açúcar adicionado.",
                "Acrescente fruta e aveia para aumentar fibras e variar o sabor.")
        };

        public static IReadOnlyList<NutritionProfile> All => Items;

        public static NutritionProfile GetById(string id)
        {
            for (var i = 0; i < Items.Length; i++)
            {
                if (Items[i].Id == id)
                {
                    return Items[i];
                }
            }

            return Items[0];
        }

        public static NutritionProfile GetAt(int index)
        {
            var safeIndex = ((index % Items.Length) + Items.Length) % Items.Length;
            return Items[safeIndex];
        }
    }
}


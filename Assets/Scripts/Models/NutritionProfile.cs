using System;

namespace NutriAR.Models
{
    public enum HealthAlertLevel
    {
        Positive,
        Attention,
        High
    }

    [Serializable]
    public sealed class NutritionProfile
    {
        public string Id;
        public string Name;
        public string Portion;
        public int Calories;
        public float Carbohydrates;
        public float Proteins;
        public float Fats;
        public float Fiber;
        public float Sugar;
        public float Sodium;
        public float SaturatedFat;
        public float GramsPerPortion;
        public string Barcode;
        public bool IsFromDatabase;
        public HealthAlertLevel AlertLevel;
        public string AlertTitle;
        public string AlertMessage;
        public string Guidance;

        public NutritionProfile(
            string id,
            string name,
            string portion,
            int calories,
            float carbohydrates,
            float proteins,
            float fats,
            float fiber,
            float sugar,
            float sodium,
            HealthAlertLevel alertLevel,
            string alertTitle,
            string alertMessage,
            string guidance,
            float saturatedFat = 0f,
            float gramsPerPortion = 100f,
            string barcode = "",
            bool isFromDatabase = false)
        {
            Id = id;
            Name = name;
            Portion = portion;
            Calories = calories;
            Carbohydrates = carbohydrates;
            Proteins = proteins;
            Fats = fats;
            Fiber = fiber;
            Sugar = sugar;
            Sodium = sodium;
            SaturatedFat = saturatedFat;
            GramsPerPortion = gramsPerPortion;
            Barcode = barcode;
            IsFromDatabase = isFromDatabase;
            AlertLevel = alertLevel;
            AlertTitle = alertTitle;
            AlertMessage = alertMessage;
            Guidance = guidance;
        }
    }
}

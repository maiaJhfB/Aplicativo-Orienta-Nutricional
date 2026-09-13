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
            string guidance)
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
            AlertLevel = alertLevel;
            AlertTitle = alertTitle;
            AlertMessage = alertMessage;
            Guidance = guidance;
        }
    }
}


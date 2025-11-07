using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBeast.ViewModels.Stats
{
    internal class StatsViewModel
    {
        // Propriedade para armazenar o nome da estatística
        public string StatName { get; set; }

        // Propriedade para armazenar o valor da estatística
        public double Value { get; set; }

        // Propriedade para armazenar a unidade da estatística
        public string Unit { get; set; }

        // Propriedade para armazenar uma descrição opcional
        public string Description { get; set; }

        // Construtor padrão
        public StatsViewModel()
        {
        }

        // Construtor com parâmetros
        public StatsViewModel(string statName, double value, string unit, string description = "")
        {
            StatName = statName;
            Value = value;
            Unit = unit;
            Description = description;
        }

        // Método para atualizar o valor da estatística
        public void UpdateValue(double newValue)
        {
            Value = newValue;
        }

        // Método para exibir os detalhes da estatística
        public override string ToString()
        {
            return $"{StatName}: {Value} {Unit} - {Description}";
        }
    }
}

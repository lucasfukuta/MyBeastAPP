using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using MyBeast.Messages;
using MyBeast.Models;
using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.Defaults; 
using SkiaSharp;
using System.Globalization;

namespace MyBeast.ViewModels.Stats
{
    public partial class StatsPageViewModel : ObservableObject
    {
        [ObservableProperty] private int totalCalories = 0;
        [ObservableProperty] private double activeTime = 0;
        [ObservableProperty] private int totalWorkouts = 0;
        [ObservableProperty] private int currentStreak = 0;

        public ISeries[] Series { get; set; }
        public Axis[] XAxes { get; set; }
        public Axis[] YAxes { get; set; }

        // --- MUDANÇA 1: Usar ObservableValue em vez de double ---
        // Isso permite que o gráfico saiba quando o valor numérico mudou
        private ObservableCollection<ObservableValue> _burnedValues;
        private ObservableCollection<ObservableValue> _consumedValues;

        public ObservableCollection<Achievement> RecentAchievements { get; set; }

        public StatsPageViewModel()
        {
            RecentAchievements = new ObservableCollection<Achievement>();

            InitializeChart();

            // Escuta Treino (Queimadas)
            WeakReferenceMessenger.Default.Register<WorkoutFinishedMessage>(this, (r, m) =>
            {
                MainThread.BeginInvokeOnMainThread(() => ProcessWorkoutData(m.Value));
            });

            // Escuta Dieta (Consumidas)
            WeakReferenceMessenger.Default.Register<DietConsumedMessage>(this, (r, m) =>
            {
                MainThread.BeginInvokeOnMainThread(() => ProcessDietData(m.Value));
            });
        }

        private void InitializeChart()
        {
            // --- MUDANÇA 2: Inicializar com ObservableValue ---
            _burnedValues = new ObservableCollection<ObservableValue> { new(0), new(0), new(0), new(0), new(0), new(0), new(0) };
            _consumedValues = new ObservableCollection<ObservableValue> { new(0), new(0), new(0), new(0), new(0), new(0), new(0) };

            // --- 1. GERAR RÓTULOS DINÂMICOS (COM DATA REAL) ---
            var labels = new string[7];
            var monday = GetMondayOfWeek(); // Pega a segunda-feira desta semana

            for (int i = 0; i < 7; i++)
            {
                var date = monday.AddDays(i);
                // Formata como "Seg 25", "Ter 26"
                // "ddd" = Dia da semana abreviado, "dd" = Dia do mês
                labels[i] = $"{date.ToString("ddd", new CultureInfo("pt-BR"))} {date.Day}";

                // Capitaliza a primeira letra (seg -> Seg)
                labels[i] = char.ToUpper(labels[i][0]) + labels[i].Substring(1);
            }

            // --- 2. CONFIGURAR AS SÉRIES ---
            Series = new ISeries[]
            {
        new LineSeries<ObservableValue>
        {
            Values = _burnedValues,
            Name = "Queimadas",
            Stroke = new SolidColorPaint(SKColors.DodgerBlue) { StrokeThickness = 3 },
            GeometryStroke = new SolidColorPaint(SKColors.DodgerBlue) { StrokeThickness = 3 },
            GeometrySize = 10,
            Fill = null
        },
        new LineSeries<ObservableValue>
        {
            Values = _consumedValues,
            Name = "Consumidas",
            Stroke = new SolidColorPaint(SKColors.Orange) { StrokeThickness = 3 },
            GeometryStroke = new SolidColorPaint(SKColors.Orange) { StrokeThickness = 3 },
            GeometrySize = 10,
            Fill = null
        }
            };

            // --- 3. CONFIGURAR EIXO X COM OS RÓTULOS REAIS ---
            XAxes = new Axis[]
            {
        new Axis
        {
            Labels = labels, // Usa o array gerado com datas
            LabelsPaint = new SolidColorPaint(SKColors.LightGray),
            TextSize = 12
        }
            };

            YAxes = new Axis[]
            {
        new Axis
        {
            LabelsPaint = new SolidColorPaint(SKColors.LightGray),
            SeparatorsPaint = new SolidColorPaint(SKColors.Gray.WithAlpha(50))
        }
            };
        }

        // --- HELPER PARA ACHAR A SEGUNDA-FEIRA ---
        private DateTime GetMondayOfWeek()
        {
            DateTime today = DateTime.Now;

            // DayOfWeek do C#: Domingo=0, Segunda=1 ... Sábado=6
            int currentDay = (int)today.DayOfWeek;

            // Queremos que a semana comece na Segunda (1).
            // Se hoje for Domingo (0), precisamos voltar 6 dias.
            // Se for Segunda (1), voltamos 0 dias.
            int daysToSubtract = (currentDay == 0) ? 6 : currentDay - 1;

            return today.AddDays(-daysToSubtract);
        }

        private void ProcessWorkoutData(WorkoutResult result)
        {
            TotalCalories += result.Calories;
            TotalWorkouts++;
            ActiveTime += result.Duration.TotalHours;

            // --- MUDANÇA 3: Atualizar a propriedade .Value ---
            int index = GetDayIndex();

            // Se o valor for nulo (segurança), assume 0
            double atual = _burnedValues[index].Value ?? 0;

            // Atualiza o valor. O LiveCharts percebe isso automaticamente.
            _burnedValues[index].Value = atual + result.Calories;

            CheckAchievements();
        }

        private void ProcessDietData(int calories)
        {
            int index = GetDayIndex();

            // Pega o valor atual (proteção contra nulo)
            double valorAtual = _consumedValues[index].Value ?? 0;

            // Calcula o novo valor
            double novoValor = valorAtual + calories;
            if (novoValor < 0)
            {
                novoValor = 0;
            }

            // Atualiza o valor final
            _consumedValues[index].Value = novoValor;
        }

        private int GetDayIndex()
        {
            int day = (int)DateTime.Now.DayOfWeek;
            // Ajuste: Domingo(0) vira índice 6. Segunda(1) vira índice 0.
            return day == 0 ? 6 : day - 1;
        }

        private void CheckAchievements()
        {
            if (TotalWorkouts == 1 && !RecentAchievements.Any())
            {
                RecentAchievements.Add(new Achievement
                {
                    Title = "Primeiro Passo",
                    Date = DateTime.Now.ToString("dd/MM"),
                    Icon = "fire_icon.png",
                    BadgeIcon = "achieved_trophy_icon.png"
                });
            }
        }
    }
}
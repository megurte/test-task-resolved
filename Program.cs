using System;
using System.Collections.Generic;
using System.Linq;


namespace Gridnine.FlightCodingTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Rules rules = new Rules();

            Console.WriteLine("Выберите правило для выборки полетов и введите соответствующее число: Основные правила\n1. Вылет до текущего момента времени\n2. Имеются сегменты с датой прилёта раньше даты вылета\n3. Общее время, проведённое на земле превышает два часа\n");
            Console.WriteLine("\nДополнительные правила\n4. Сегментов за перелет больше n числа\n5. Ближайший перелет (Вылет)\n6. Вывести продолжительность перелетов\n7. Вывести все расписание перелетов и сегментов\n");
            int num = Convert.ToInt32(Console.ReadLine());
            rules.RulesApply(num);

            Pause();
        }

        static void Pause()
        {
            Console.Write("Нажмите любую клавишу для продолжения...");
            Console.ReadKey(true);
        }
    }
 
    
    public class Rules
    {
        public void RulesApply(int rule_num)
        {
            FlightBuilder flightbuilder = new FlightBuilder();
            IList<Flight> flights = flightbuilder.GetFlights();

            switch (rule_num) 
            {
                case 1:
                    //вылет до текущего момента времени

                    List<TimeSpan> summatorDate1 = new List<TimeSpan>();
                    foreach (Flight flight in flights)
                    {
                        TimeSpan timeSpan = new TimeSpan();
                        foreach (Segment segment in flight.Segments)
                        {
                            timeSpan += segment.ArrivalDate.Subtract(segment.DepartureDate);
                        }
                        if (timeSpan < TimeSpan.Zero)
                            summatorDate1.Add(timeSpan);
                    }
                    foreach (TimeSpan timeSpan1 in summatorDate1)
                        Console.WriteLine("Общее время перелета: " + timeSpan1);

                    break;
                case 2:
                    //имеются сегменты с датой прилёта раньше даты вылета

                    foreach (Flight flight in flights)
                    {
                        foreach (Segment segment in flight.Segments)
                        {
                            if (segment.ArrivalDate.CompareTo(segment.DepartureDate) < 0)
                            {
                                Console.WriteLine("Сегмент: \n" + segment.DepartureDate + "\n" + segment.ArrivalDate);
                            }
                        }
                    }

                    break;
                case 3:
                    //общее время, проведённое на земле превышает два часа 
                    //(время на земле — это интервал между прилётом одного сегмента и вылетом следующего за ним)

                    foreach (Flight flight in flights)
                    {
                        for (int i = 0; i < flight.Segments.Count - 1; i++)
                        {
                            if ((flight.Segments[i + 1].DepartureDate.Hour - flight.Segments[i].ArrivalDate.Hour) < 2)
                            {
                                Console.WriteLine(flight);
                                Console.WriteLine("Сегмент: \n" + flight.Segments[i + 1].DepartureDate + "\n" + flight.Segments[i].ArrivalDate);
                            }
                        }
                    }
                    

                        break;
                case 4:
                    //Сегментов за перелет больше n числа

                    Console.WriteLine("Введите количество перелетов: ");
                    int num_segments = Convert.ToInt32(Console.ReadLine());

                    for (int i = 0; i < flights.Count; i++)
                    {
                        if (num_segments <= flights[i].Segments.Count)
                            Console.WriteLine(flights[i]);
                    }

                    break;
                case 5:                    
                    //Ближайший перелет (Вылет)

                    DateTime temp;
                    for (int i = 0; i < flights.Count; i++)
                    {
                        for (int j = i + 1; j < flights.Count; j++)
                        {
                            if (flights[i].Segments[0].DepartureDate > flights[j].Segments[0].DepartureDate)
                            {
                                if (DateTime.Now < flights[i].Segments[0].DepartureDate)
                                {
                                    temp = flights[i].Segments[0].DepartureDate;
                                    flights[i].Segments[0].DepartureDate = flights[j].Segments[0].DepartureDate;
                                    flights[j].Segments[0].DepartureDate = temp;
                                }                               
                            }
                        }                     
                    }
                    Console.WriteLine("Ближайший рейс отправляется: " + flights[0].Segments[0].DepartureDate);
                    break;
                case 6:
                    //Вывести продолжительность перелета

                    List<TimeSpan> summatorDate = new List<TimeSpan>();
                    foreach (Flight flight in flights)
                    {
                        TimeSpan timeSpan = new TimeSpan();
                        foreach (Segment segment in flight.Segments)
                        {
                            timeSpan += segment.ArrivalDate.Subtract(segment.DepartureDate);
                        }
                        summatorDate.Add(timeSpan);
                    }

                    foreach (TimeSpan timeSpan1 in summatorDate)
                        Console.WriteLine("Общее время перелета: " + timeSpan1);
        
                    break;
                case 7:                 
                    //Вывести все расписание перелетов

                    for (int i = 0; i < flights.Count; i++)
                    {
                        for (int j = 0; j < flights[j].Segments.Count; j++)
                        {
                            foreach (Segment segment in flights[i].Segments)
                            {
                                Console.WriteLine(i + 1 + " рейс:");
                                Console.WriteLine(segment.DepartureDate + "\n" + segment.ArrivalDate);
                            }
                        }
                    }

                    break;
            }
        }
    }



    public class FlightBuilder
    {
        private DateTime _threeDaysFromNow;

        public FlightBuilder()
        {
            _threeDaysFromNow = DateTime.Now.AddDays(3);
        }

        public IList<Flight> GetFlights()
        {
            return new List<Flight>
                       {
                           //A normal flight with two hour duration
			               CreateFlight(_threeDaysFromNow, _threeDaysFromNow.AddHours(2)),

                           //A normal multi segment flight
			               CreateFlight(_threeDaysFromNow, _threeDaysFromNow.AddHours(2), _threeDaysFromNow.AddHours(3), _threeDaysFromNow.AddHours(5)),
                           
                           //A flight departing in the past
                           CreateFlight(_threeDaysFromNow.AddDays(-6), _threeDaysFromNow),

                           //A flight that departs before it arrives
                           CreateFlight(_threeDaysFromNow, _threeDaysFromNow.AddHours(-6)),

                           //A flight with more than two hours ground time
                           CreateFlight(_threeDaysFromNow, _threeDaysFromNow.AddHours(2), _threeDaysFromNow.AddHours(5), _threeDaysFromNow.AddHours(6)),

                            //Another flight with more than two hours ground time
                           CreateFlight(_threeDaysFromNow, _threeDaysFromNow.AddHours(2), _threeDaysFromNow.AddHours(3), _threeDaysFromNow.AddHours(4), _threeDaysFromNow.AddHours(6), _threeDaysFromNow.AddHours(7))
                       };
        }

        private static Flight CreateFlight(params DateTime[] dates)
        {
            if (dates.Length % 2 != 0) throw new ArgumentException("You must pass an even number of dates,", "dates");

            var departureDates = dates.Where((date, index) => index % 2 == 0);
            var arrivalDates = dates.Where((date, index) => index % 2 == 1);

            var segments = departureDates.Zip(arrivalDates,
                                              (departureDate, arrivalDate) =>
                                              new Segment { DepartureDate = departureDate, ArrivalDate = arrivalDate }).ToList();

            return new Flight { Segments = segments };
        }
    }

    public class Flight
    {
        public IList<Segment> Segments { get; set; }
    }

    public class Segment
    {
        public DateTime DepartureDate { get; set; }
        public DateTime ArrivalDate { get; set; }
    }
}


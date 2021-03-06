﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Linq;

namespace GraphMaker.TFSAlgorithm
{
    public  class SimulatedAnnealing
    {
        private  int EdgesCount { get; set; }
        private  double Proba { get; set; }
        private  double Alpha {get;set;}
        private  double Temperature { get; set; }
        private  double Epsilon { get; set; }
        private  double Delta { get; set; }
        public  double CurrentDistance { get; set; }

        private Random rand;

        private  List<int> currentOrder = new List<int>();
        private  List<int> nextOrder = new List<int>();

        private  List<SilverlightEdge> _edges;
        private double[,] _distancesArray;
 
        public  List<int>  CalculateInit(List<SilverlightEdge> current,double [,] distancesArray,double alpha=0.999,double temp = 400.0,double epsilon = 0.001)
        {
            rand = new Random(DateTime.Now.Millisecond);
            EdgesCount = current.Count;
            _edges = current;

            for (int i = 0; i < current.Count; i++)
            {
                currentOrder.Add(i);
            }
            _distancesArray = distancesArray;
            Alpha = alpha;
            Temperature = temp;
            Epsilon = epsilon;
            currentOrder = ShrinkWrap.CreateInitialorder(current);
            CurrentDistance = CalculateDistance(currentOrder, _distancesArray);

            return currentOrder;
        }

        public  List<int> Calculate(int nrOfIterations=10,bool best =true)
        {
            int iteration= 0;
            double delta = 0.0;
            double proba = 0.0;

            for (int k = 0; k < nrOfIterations; )
            {
                CreateRandomOrder();
                iteration++;
                delta = CalculateDistance(nextOrder, _distancesArray) -CurrentDistance;
                if (delta < 0)
                {
                    Copy(currentOrder,nextOrder);
                    CurrentDistance = delta + CurrentDistance;
                }
                else
                {
                    proba = rand.NextDouble();
                    if (proba < Math.Exp(-delta / Temperature))
                    {
                        Copy(currentOrder, nextOrder);
                        CurrentDistance = delta + CurrentDistance;
                    }
                }
                Temperature *= Alpha;
                if ((Temperature < Epsilon))
                {
                    return currentOrder;
                }

                if (!best)
                {
                    k++;
                }

            }

            return currentOrder;
        }

        private  void Copy(List<int> currentOrder, List<int> nextOrder)
        {
            currentOrder.Clear();

            foreach (int i in nextOrder)
            {
                currentOrder.Add(i);
            }
        }

        private  void CreateRandomOrder()
        {
            nextOrder.Clear();

            foreach (int i in currentOrder)
            {
                nextOrder.Add(i);
            }

            int index1 = (int)(rand.Next(EdgesCount - 1));
            int index2 = (int)(rand.Next(EdgesCount - 1));
            int temp = nextOrder[index1];
            nextOrder[index1] = nextOrder[index2];
            nextOrder[index2] = temp;
        }

        private  double CalculateDistance(List<int> order, double [,] distanceArray)
        {

            double distance = 0.0;

            for (int i = 0; i < order.Count-1; i++)
            {
                distance += distanceArray[order[i], order[i + 1]];
            }

            distance += distanceArray[order[order.Count - 1], order[0]];

            return distance;
        }
    }
}

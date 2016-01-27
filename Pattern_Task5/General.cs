using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pattern_Task5
{
    class General
    {

        List<List<double>> Data;
        List<KeyValuePair<int, int>> TrainingIntervals;
        List<KeyValuePair<int, int>> TestingIntervals;
        List<List<int>> Confusion = new List<List<int>>();
        int Feature_no = 4;
        int Class_no = 3;
        int h = 3; //user input
        int windowSamples = 0;
        int[] k;
        int[] result;

        public General(string FileName)
        {
            Data = new List<List<double>>();
            this.TestingIntervals = new List<KeyValuePair<int, int>>();
            this.TrainingIntervals = new List<KeyValuePair<int, int>>();
            k = new int[Class_no];
            result = new int[Class_no];
            readfile(FileName);
        }
        public General()
        {
            Data = new List<List<double>>();
            this.TestingIntervals = new List<KeyValuePair<int, int>>();
            this.TrainingIntervals = new List<KeyValuePair<int, int>>();
        }

        public List<List<double>> readfile(string FileName)
        {
            string[] tmp;
            FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
            var streamReader = new StreamReader(fs);
            string line = "";

            int i = 0; // number of samples
            while ((line = streamReader.ReadLine()) != null)
            {
                Data.Add(new List<double>());

                tmp = line.Split(',');
                for (int j = 0; j < Feature_no; j++)
                {
                    this.Data[i].Add(double.Parse(tmp[j]));
                }
                i++;

            }
            return Data;
        }

        public KeyValuePair<List<KeyValuePair<int, int>>, List<KeyValuePair<int, int>>> calculate_interval(double t_ds_total, int intervals)
        {
            //given how many training samples from all the dataset ex: 0.4 of 150 = 60 sample for training
            //given intervals divides the given training samples into no of intervals ex: 3 intervals for 60 samples=20
            List<KeyValuePair<int, int>> TrS_intervals = new List<KeyValuePair<int, int>>();
            int training_ds_total = (int)(t_ds_total * (double)this.Data.Count);
            int testing_ds_total = testing_ds_total = this.Data.Count - training_ds_total; ;
            for (int i = 0; i < intervals; i++)
            {
                if (i == 0)
                    TrS_intervals.Add(new KeyValuePair<int, int>(i, (i + 1) * (training_ds_total / intervals) - 1));
                else
                    TrS_intervals.Add(new KeyValuePair<int, int>(i * (training_ds_total / intervals + testing_ds_total / intervals)
                        , i * (training_ds_total / intervals + testing_ds_total / intervals) + training_ds_total / intervals - 1));
            }

            //testing intervals 
            List<KeyValuePair<int, int>> TeS_intervals = new List<KeyValuePair<int, int>>();
            for (int i = 0; i < TrS_intervals.Count; i++)
            {
                if (i + 1 != TrS_intervals.Count)
                    TeS_intervals.Add(new KeyValuePair<int, int>(TrS_intervals[i].Value + 1, TrS_intervals[i + 1].Key - 1));
                else
                    TeS_intervals.Add(new KeyValuePair<int, int>(TrS_intervals[i].Value + 1, (TeS_intervals[0].Value - TeS_intervals[0].Key) + TrS_intervals[i].Value + 1));
            }
            return new KeyValuePair<List<KeyValuePair<int, int>>, List<KeyValuePair<int, int>>>(TrS_intervals, TeS_intervals);
        }

        public void setIntervals(KeyValuePair<List<KeyValuePair<int, int>>, List<KeyValuePair<int, int>>> intervals)
        {

            this.TrainingIntervals.AddRange(intervals.Key);
            this.TestingIntervals.AddRange(intervals.Value);
        }

        public void parzenWindowClassifier()
        {


            for (int i = 0; i < this.TestingIntervals.Count; i++)//classes
            {
                for (int j = this.TestingIntervals[i].Key; j <= this.TestingIntervals[i].Value; j++)//testing samples of class i
                {
                    for (int n = 0; n < Class_no; n++)
                        k[n] = 0;

                    for (int m = 0; m < this.TrainingIntervals.Count; m++)//classes 
                    {

                        for (int n = this.TrainingIntervals[m].Key; n <= this.TrainingIntervals[m].Value; n++)//training samples of class j
                        {
                            parzen(m, Data[j], Data[n]);  // parzen function parzen(count , array of IDs[],classID ,testing sample , training sample)
                        }
                    }
                    result[k.ToList().IndexOf(k.Max())]++;
                }
            }

        }

        void parzen(int classID, List<double> testingSample, List<double> trainingSample)
        {

            if (isInsideWindow(testingSample, trainingSample))
            {
                k[classID]++;
            }
        }

        bool isInsideWindow(List<double> testingSample, List<double> trainingSample)
        {
            for (int i = 0; i < Feature_no; i++)
            {
                if (Math.Abs(testingSample[i] - trainingSample[i]) >= (h / 2))
                    return false;
            }
            return true;
        }


        public double getAccuracy()
        {


            double sum = 0;
            for (int i = 0; i < result.Length; i++)
            {

                //note di mesh confusion di el diagonal bas
                if ((TestingIntervals[0].Value - TestingIntervals[0].Key + 1) > result[i])
                {

                    sum += result[i];
                }
                else
                    sum += (TestingIntervals[0].Value - TestingIntervals[0].Key + 1);

            }
            sum /= 90;
            sum *= 100;
            return sum;

        }

    }
}
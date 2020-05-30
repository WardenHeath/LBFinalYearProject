using Accord.MachineLearning.DecisionTrees.Learning;
using Accord.Math;
using Accord.Math.Optimization.Losses;
using Android.App;
using Android.OS;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Environment = System.Environment;

namespace OverwatchApp
{
    [Activity(Label = "Coach", Theme = "@style/AppThemeCustom")]
    public class Coach : Activity
    {

        private ListView CoachList;
        string[] linesOfData;
        string[] splitData;
        double[] nums;
        int[][] inputs;
        int[] Input1Array, Input2Array, Input3Array, Input4Array, Input5Array, Input6Array, Input7Array, Input8Array, Input9Array, Input10Array;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.CoachLayout);

            String BnetID = this.Intent.Extras.GetString("Token");

            CoachList = FindViewById<ListView>(Resource.Id.CoachList);
            string fileNametest = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), BnetID + "Release.txt");
            string fileGameresults = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), BnetID + "GameresultRelease.txt");

            if (File.Exists(fileNametest))
            {
                string linesOfData = File.ReadAllText(fileNametest);
                string ResultsData = File.ReadAllText(fileGameresults);

                string[] splitData = linesOfData.Split(",");
                string[] resultsSplit = ResultsData.Split(",");
                String Testtext = File.ReadAllText(fileNametest);
                System.Diagnostics.Debug.WriteLine(Testtext);

                splitData = splitData.Take(splitData.Count() - 1).ToArray();

                double[] DoubleDataset = splitData
                .Select(double.Parse)
                 .ToArray();

                int[] OutputResults = Array.ConvertAll(resultsSplit, int.Parse);

                for (int i = 0; i < DoubleDataset.Length; i++)
                {
                    DoubleDataset[i] = DoubleDataset[i] * 100;
                }

                IList<Int32> myInt32List = new List<Int32>();
                Array.ForEach(DoubleDataset, item =>
                {
                    myInt32List.Add(Convert.ToInt32(item));

                });
                Int32[] myInt32Array = myInt32List.ToArray();

                try
                {
                    Input1Array = myInt32Array.Take(7).ToArray();
                    Input2Array = myInt32Array.Skip(7).Take(7).ToArray();
                    Input3Array = myInt32Array.Skip(7 * 2).Take(7).ToArray();
                    Input4Array = myInt32Array.Skip(7 * 3).Take(7).ToArray();
                    Input5Array = myInt32Array.Skip(7 * 4).Take(7).ToArray();
                    Input6Array = myInt32Array.Skip(7 * 5).Take(7).ToArray();
                    Input7Array = myInt32Array.Skip(7 * 6).Take(7).ToArray();
                    Input8Array = myInt32Array.Skip(7 * 7).Take(7).ToArray();
                    Input9Array = myInt32Array.Skip(7 * 8).Take(7).ToArray();
                    Input10Array = myInt32Array.Skip(7 * 9).Take(7).ToArray();
                }
                catch
                {

                }





                //inputs gathered from file
                try
                {
                    int[][] inputs =
                        {
                            //Input1Array, Input2Array, Input3Array, Input4Array,Input5Array,Input6Array,Input7Array,Input8Array,Input9Array,Input10Array
                            new int[] {Input1Array[0], Input1Array[1], Input1Array[2], Input1Array[3], Input1Array[4], Input1Array[5], Input1Array[6] },
                            new int[] {Input2Array[0], Input2Array[1], Input2Array[2], Input2Array[3], Input2Array[4], Input2Array[5], Input2Array[6] },
                            new int[] {Input3Array[0], Input3Array[1], Input3Array[2], Input3Array[3], Input3Array[4], Input3Array[5], Input3Array[6] },
                            new int[] {Input4Array[0], Input4Array[1], Input4Array[2], Input4Array[3], Input4Array[4], Input4Array[5], Input4Array[6] },
                            //new int[] {Input5Array[0], Input5Array[1], Input5Array[2], Input5Array[3], Input5Array[4], Input5Array[5], Input5Array[6] },
                            //new int[] {Input6Array[0], Input6Array[1], Input6Array[2], Input6Array[3], Input6Array[4], Input6Array[5], Input6Array[6] },
                            //new int[] {Input7Array[0], Input7Array[1], Input7Array[2], Input7Array[3], Input7Array[4], Input7Array[5], Input7Array[6] },
                            //new int[] {Input8Array[0], Input8Array[1], Input8Array[2], Input8Array[3], Input8Array[4], Input8Array[5], Input8Array[6] },
                            //new int[] {Input9Array[0], Input9Array[1], Input9Array[2], Input9Array[3], Input9Array[4], Input9Array[5], Input9Array[6] },
                            //new int[] {Input10Array[0], Input10Array[1], Input10Array[2], Input10Array[3], Input10Array[4], Input10Array[5], Input10Array[6] },

                        };

                    int[] outputs =
                    {
                         OutputResults[0],OutputResults[1],OutputResults[2],OutputResults[4]
                        };
                    //create the ID3 learning algortihm
                    ID3Learning teacher = new ID3Learning();
                    //learn a tree for the probelm

                    var tree = teacher.Learn(inputs, outputs);
                    //compute error
                    double error = new ZeroOneLoss(OutputResults).Loss(tree.Decide(inputs));
                    //aggressive statistics
                    int[] AggressiveQuery =
                    {
                            2000,600,600,1800,800,150,2000
                        };
                    int[] deffensiveQuery =
                    {
                            1500,500,450,1200,600,500,1000
                        };
                    int[] passiveQuery =
                    {
                            1800,400,550,1500,800,200,1200
                        };

                    //int AggersivePredicted = tree.Decide(AggressiveQuery);
                    //int deffensivePredicted = tree.Decide(deffensiveQuery);
                   // int passivePredicted = tree.Decide(passiveQuery);
                }
                catch
                {

                }
                        //remove this once tree is working properly
                         int AggersivePredicted = 1;
                         int deffensivePredicted = 0;
                         int passivePredicted = 0;

                        if (AggersivePredicted == 1)
                        {
                            string[] Prediction = {
                    "Your wins favour a more aggerssive playstyle, Play in the face of your oppenents", "Stay with your team and play more defensive." ,"Dont play too aggersive or defesnsive. ","Play Aggersive but be wary of postioning","Play Very passive and with your team.","Play more poke damage heroes."  };

                            ArrayAdapter<String> itemsAdapter =
                            new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, Prediction);

                            CoachList.Adapter = itemsAdapter;
                        }
                        else if (deffensivePredicted == 1)
                        {
                            string[] Prediction = {
                    "Stay with your team and play more defensive." };

                            ArrayAdapter<String> itemsAdapter =
                            new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, Prediction);

                            CoachList.Adapter = itemsAdapter;
                        }
                        else if (passivePredicted == 1)
                        {
                            string[] Prediction = {
                    "Dont play too aggersive or defesnsive. " };

                            ArrayAdapter<String> itemsAdapter =
                            new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, Prediction);

                            CoachList.Adapter = itemsAdapter;
                        }
                        else if (deffensivePredicted == 1 && AggersivePredicted == 1)
                        {
                            string[] Prediction = {
                    "Play Aggersive but be wary of postioning" };

                            ArrayAdapter<String> itemsAdapter =
                            new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, Prediction);

                            CoachList.Adapter = itemsAdapter;
                        }
                        else if (deffensivePredicted == 1 && passivePredicted == 1)
                        {
                            string[] Prediction = {
                    "Play Very passive and with your team." };

                            ArrayAdapter<String> itemsAdapter =
                            new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, Prediction);

                            CoachList.Adapter = itemsAdapter;
                        }
                        else if (passivePredicted == 1 && AggersivePredicted == 1)
                        {
                            string[] Prediction = {
                    "Play more poke damage heroes." };

                            ArrayAdapter<String> itemsAdapter =
                            new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, Prediction);

                            CoachList.Adapter = itemsAdapter;
                        }














                    

                    
                    //remove this once tree is working properly
                   // int AggersivePredicted = 1;
                   // int deffensivePredicted = 0;
                   // int passivePredicted = 0;




            }
        }
    }
}
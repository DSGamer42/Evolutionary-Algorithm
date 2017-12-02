using System;
using System.Linq;

namespace Evolution
{
    public class Evo
    {
        public Action<string> DebugLogAction;

        public float CrossoverRate;
        public float MutationRate;
        public int PopulationSize;
        public int GenomLength;

        public readonly Func<bool[], int> FitnessFunc;
        public Genom[] Genoms;
        private Genom[] _tmpGenoms; //to be able to flip

        private Random r = new Random();
        private readonly SimpleRatedPicker<Genom> _picker;

        private int _emergencyGenerations;
        private int _restartCount;
        
        public Evo(int populationSize, Func<bool[], int> fitnessFunc, int genomLength, float crossoverRate = 0.7f, float mutationRate = 0.001f)
        {
            _picker = new SimpleRatedPicker<Genom>(r);

            CrossoverRate = crossoverRate;
            MutationRate = mutationRate;
            PopulationSize = populationSize;
            FitnessFunc = fitnessFunc;
            GenomLength = genomLength;
        }

        public bool[][] FittestElements(int count)
        {
            return Genoms.OrderByDescending(g => FitnessFunc(g.Code)).Take(count).Select(g => g.Code).ToArray();
        }

        public bool[] FittestElement()
        {
            int highestValue = 0;
            bool[] fittestGenomCode = null;

            foreach (Genom g in Genoms)
            {
                if (FitnessFunc(g.Code) > highestValue)
                {
                    fittestGenomCode = g.Code;
                    highestValue = FitnessFunc(g.Code);
                }
            }
            bool[] tmp = new bool[GenomLength];
            if (fittestGenomCode != null)
            {
                fittestGenomCode.CopyTo(tmp, 0);
            }
            else
            {
#if DEBUG
                Console.WriteLine("No Viable Result. Try different Parameters");
#endif
                return null;
            }
        
            return tmp;
        }

        public void CalculateGenerations(int generations)
        {
            try
            {
                _emergencyGenerations = generations;
                StartPop();
                for (int i = 0; i < generations; i++)
                {
                    ProcessGeneration();
                }
            }
            catch (UnableToPickException)
            {
                _restartCount++;

#if DEBUG
                Console.WriteLine("Was unable to pick, probably due to all chances zero. Restarting");
                
                if (_restartCount > 100)
                {
                    Console.WriteLine("Restarting very often, consider choosing different params");
                    _restartCount = 0;
                }

                CalculateGenerations(_emergencyGenerations);
#endif
                if (DebugLogAction != null)
                {
                    DebugLogAction("Was unable to pick, probably due to all chances zero. Restarting");

                    if (_restartCount > 100)
                    {
                        DebugLogAction("Restarting very often... consider choosing different Parameters");
                        _restartCount = 0;
                    }
                }
            }
            
        }

        private bool[] Recombine(Genom one, Genom two, out bool[] resultTwo)
        {
            bool[]codeOne = new bool[GenomLength];
            bool[]codeTwo = new bool[GenomLength];

            int skip = r.Next(0, GenomLength);
            for (int i = 0; i < GenomLength; i++)
            {
                if (i > skip)
                {
                    codeOne[i] = two.Code[i];
                    codeTwo[i] = one.Code[i];
                }
                else
                {
                    codeOne[i] = one.Code[i];
                    codeTwo[i] = two.Code[i];
                }
                    
            }
            resultTwo = codeTwo;
            return codeOne;
        }

        private bool[] MutateGenomCode(bool[] one)
        {
            bool[] tmp = new bool[one.Length];
            for (int i = 0; i < GenomLength; i++)
            {
                tmp[i] = (r.NextDouble() <= MutationRate) ? !one[i] : one[i];
            }
            return tmp;
        }

        private void StartPop()
        {
            Genoms = new Genom[PopulationSize];
            _tmpGenoms = new Genom[PopulationSize];
            for (int i = 0; i < Genoms.Length; i++)
            {
                Genoms[i] = new Genom(RandomGenomCode(GenomLength));
                _tmpGenoms[i] = new Genom();
            }
        }

        private void ProcessGeneration()
        {
            int n = 0; // how many Genoms have been added to the new Pop
            FillPicker();

            while (n < PopulationSize)
            {
                if (r.NextDouble() <= CrossoverRate && n+1<PopulationSize)
                {
                    bool[] tmp;
                    _tmpGenoms[n].Code = MutateGenomCode(Recombine(_picker.Pick(), _picker.Pick(), out tmp)); //return of the first recombine result is checked for mutation instantly
                    _tmpGenoms[n + 1].Code = MutateGenomCode(tmp);
                    n += 2;
                }
                else
                {
                    _tmpGenoms[n].Code = MutateGenomCode(_picker.Pick().Code);
                    n++;
                }
            }

            Genom[] swapTmp = Genoms; //swap
            Genoms = _tmpGenoms;
            _tmpGenoms = swapTmp;
        }

        private void FillPicker()
        {
            _picker.Clear();
            foreach (Genom g in Genoms)
            {
                _picker.Add(FitnessFunc(g.Code), g);
            }
        }

        private bool[] RandomGenomCode(int length)
        {
            bool[] toReturn = new bool[length];

            for (int i = 0; i < length; i++)
            {
                toReturn[i] = r.Next(0, 1 + 1) != 0;
            }
            return toReturn;
        }
    }

    public class Genom
    {
        public bool[] Code;

        public Genom(bool[] code)
        {
            Code = code;
        }

        public Genom() { }
    }
}

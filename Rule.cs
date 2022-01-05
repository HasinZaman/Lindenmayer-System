using System;
using System.Collections.Generic;

namespace LSystem
{
    /// <summary>
    ///     Interface defines the required methods inorder to create L System Production Rule
    /// </summary>
    public interface IRule
    {
        /// <summary>
        ///     product rule defines the output
        /// </summary>
        Sentence productionRule { get;}

        /// <summary>
        ///     variable defines that triggers the production rule
        /// </summary>
        char var { get;}

        /// <summary>
        ///     replace method writes new alphabet symbols in output
        /// </summary>
        /// <param name="point"></param>
        /// <param name="output"></param>
        void replace(Sentence.Enumerator point, Sentence output);
    }

    /// <summary>
    ///     Rule class creates the basic L-System Production Rule.
    /// </summary>
    public class Rule : IRule
    {
        /// <summary>
        ///     product rule defines the output
        /// </summary>
        public Sentence productionRule
        {
            get;
            private set;
        }

        /// <summary>
        ///     variable defines that triggers the production rule
        /// </summary>
        public char var
        {
            get;
            private set;
        }

        /// <summary>
        ///     Constructor creates rule Instance
        /// </summary>
        /// <param name="var"></param>
        /// <param name="productionRule"></param>
        public Rule(char var, Sentence productionRule)
        {
            this.var = var;
            this.productionRule = productionRule;
        }

        /// <summary>
        ///     replace method writes new alphabet symbols in output
        /// </summary>
        /// <param name="point"></param>
        /// <param name="output"></param>
        public void replace(Sentence.Enumerator point, Sentence output)
        {
            if(point.Current == this.var)
            {
                output.InsertSentence(output.Count, productionRule);
            }
            else
            {
                output.Add(point.Current);
            }
        }
    }

    /// <summary>
    ///     StochasticRule class create L-System Production Rule using Stochastic grammar. In which rules are based on probability and pseudo random algorthims.
    /// </summary>
    public class StochasticRule : IRule
    {
        /// <summary>
        ///     rules define rules relative to the rule weight
        /// </summary>
        private SortedList<double, Sentence> rules = new SortedList<double, Sentence>();


        /// <summary>
        ///     product rule defines the output
        /// </summary>
        public Sentence productionRule
        {
            get
            {
                double tmp = random.NextDouble();
                for(int i1 = 0; i1 < rules.Count; i1++)
                {
                    if(tmp <= rules.Keys[i1])
                    {
                        return rules[rules.Keys[i1]];
                    }
                }
                return rules[0];
            }
        }

        /// <summary>
        ///     variable defines that triggers the production rule
        /// </summary>
        public char var
        {
            get;
            private set;
        }

        private System.Random random;

        /// <summary>
        ///     Constructor creates a StochasticRule method
        /// </summary>
        /// <param name="var"></param>
        /// <param name="seed"></param>
        public StochasticRule(char var,  int seed)
        {
            this.var = var;
            this.random = new System.Random(seed);
        }

        /// <summary>
        ///     addRule adds a product rule based on probability
        /// </summary>
        /// <param name="productRule"></param>
        /// <param name="probability"></param>
        /// <returns>bool based on whether rule was added</returns>
        public bool addRule(Sentence productRule, double probability)
        {
            if(!this.rules.ContainsKey(probability) && probability <= 1)
            {
                this.rules.Add(probability, productRule);
                return true;
            }
            return false;
        }

        /// <summary>
        ///     replace method writes new alphabet symbols in output
        /// </summary>
        /// <param name="point"></param>
        /// <param name="output"></param>
        public void replace(Sentence.Enumerator point, Sentence output)
        {
            if (point.Current == this.var)
            {
                output.InsertSentence(output.Count, productionRule);
            }
            else
            {
                output.Add(point.Current);
            }
        }
    }

    /// <summary>
    ///     ContextSensitiveRule class create L-System Production Rule using context sensitive grammar. In which rules are applied based on the alphabet symbols left and right variable.
    /// </summary>
    public class ContextSensitiveRule : IRule
    {
        /// <summary>
        ///     product rule defines the output
        /// </summary>
        public Sentence productionRule
        {
            get;
            private set;
        }

        /// <summary>
        ///     variable defines that triggers the production rule
        /// </summary>
        public char var
        {
            get;
            private set;
        }

        /// <summary>
        ///     The required chars left of variable required inorder to for rule to applied.
        ///     contextLeft = [l1, l2, l3]
        ///     l1, l2, l3, var, r1, r2, r3
        /// </summary>
        private char[] contextLeft;

        /// <summary>
        ///     The required chars right of variable required inorder to for rule to applied.
        ///     contextRight = [r1, r2, r3]
        ///     l1, l2, l3, var, r1, r2, r3
        /// </summary>
        private char[] contextRight;

        /// <summary>
        ///     Constructor creates instance of ContextSensitiveRule
        /// </summary>
        /// <param name="var"></param>
        /// <param name="productionRule"></param>
        /// <param name="contextLeft"></param>
        /// <param name="contextRight"></param>
        public ContextSensitiveRule(char var, Sentence productionRule, char[] contextLeft, char[] contextRight)
        {
            this.var = var;
            this.productionRule = productionRule;

            if(contextLeft != null)
            {
                this.contextLeft = contextLeft;
            }
            else
            {
                this.contextLeft = new char[0];
            }

            if (contextRight != null)
            {
                this.contextRight = contextRight;
            }
            else
            {
                this.contextRight = new char[0];
            }
        }

        /// <summary>
        ///     replace method writes new alphabet symbols in output
        /// </summary>
        /// <param name="point"></param>
        /// <param name="output"></param>
        public void replace(Sentence.Enumerator point, Sentence output)
        {
            if (point.Current == this.var && leftCheck(point) && rightCheck(point))
            {
                output.InsertSentence(output.Count, productionRule);
            }
            else
            {
                output.Add(point.Current);
            }
        }

        /// <summary>
        ///     private utility method used to check the context left of variable
        /// </summary>
        /// <param name="point"></param>
        /// <returns>bool of leftCheck</returns>
        private bool leftCheck(Sentence.Enumerator point)
        {
            if(this.contextLeft.Length == 0)
            {
                return true;
            }

            int i1 = 0;
            bool cond = true;

            bool forCheck = point.MovePrev();
            for (; i1 < this.contextLeft.Length && forCheck; i1++)
            {
                if(point.Current != this.contextLeft[this.contextLeft.Length - 1 - i1])
                {
                    cond = false;
                    break;
                }

                if(i1 + 1 < this.contextLeft.Length)
                {
                    forCheck = point.MovePrev();
                }
            }

            for(; 0 < i1; i1--)
            {
                point.MoveNext();
            }

            return cond && forCheck;
        }

        /// <summary>
        ///     private utility method used to check the context right of variable
        /// </summary>
        /// <param name="point"></param>
        /// <returns>bool of rightCheck</returns>
        private bool rightCheck(Sentence.Enumerator point)
        {
            if (this.contextRight.Length == 0)
            {
                return true;
            }

            int i1 = 0;
            bool cond = true;

            bool forCheck = point.MoveNext();
            for (; i1 < this.contextRight.Length && forCheck; i1++)
            {
                if (point.Current != this.contextRight[i1])
                {
                    cond = false;
                    break;
                }

                if (i1 + 1 < this.contextRight.Length)
                {
                    forCheck = point.MoveNext();
                }
            }

            for (; 0 < i1; i1--)
            {
                point.MovePrev();
            }

            return cond && forCheck;
        }
    }
}
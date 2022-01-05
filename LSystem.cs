using System.Collections.Generic;

/// <summary>
///     Lindenmayer system (L-System) is a mathematical repersentation to model the behaviour of plant cells. However, it can be used to recusively simulate the growth of fractals, vareity of other organisims and etc.
///     L-System is defined by an Alphabet, Axiom and Production rules. In which the Alphabet are symbols in a sentence. Variables are symbols that are replaced between positions. While, constants are not replace between positions. Axiom defines the intial 0th position. Product rules define how variables change between positions. Positions refer to the end state after n recursive runs using the product rules.
/// </summary>
namespace LSystem
{
    /// <summary>
    ///     LSystem class handles the state of the current and next sentences in a LSystem
    /// </summary>
    public class LSystem
    {
        /// <summary>
        ///    Axiom stores the inital state of the L-System
        /// </summary>
        public Sentence axiom
        {
            get;
            private set;
        }

        /// <summary>
        ///     Rules stores the production rule for each variable. Symbols that are not in rules instances are considered constants.
        /// </summary>
        private Dictionary<char, IRule> rules = new Dictionary<char, IRule>();

        /// <summary>
        ///     sentence stores the current sentence
        /// </summary>
        private Sentence sentence;

        /// <summary>
        ///     lastSentence is a utility instance used to store the last sentence and tempoary sentence to store symbols for the next sentence
        /// </summary>
        private Sentence lastSentence;

        /// <summary>
        ///     Char array of the current sentence
        /// </summary>
        public char[] current
        {
            get
            {
                char[] tmp = new char[sentence.Count];
                int i1 = 0;
                foreach(char c1 in sentence)
                {
                    tmp[i1] = c1;
                    i1++;
                }
                return tmp;
            }
        }

        /// <summary>
        ///     Constructor sets up LSystem
        /// </summary>
        /// <param name="axiom"></param>
        public LSystem(char[] axiom)
        {
            this.axiom = new Sentence(axiom);
            this.sentence = this.axiom;
            this.lastSentence = new Sentence();
        }

        /// <summary>
        ///     Constructor sets up LSystem
        /// </summary>
        /// <param name="axiom"></param>
        public LSystem(string axiom)
        {
            this.axiom = new Sentence(axiom.ToCharArray());
            this.sentence = new Sentence(axiom.ToCharArray());
            this.lastSentence = new Sentence();
        }

        /// <summary>
        ///     Constructor sets up LSystem
        /// </summary>
        /// <param name="axiom"></param>
        public LSystem(Sentence axiom)
        {
            this.axiom = axiom;
            this.sentence = new Sentence();
            this.sentence.InsertSentence(0, axiom.Clone());
            this.lastSentence = new Sentence();
        }

        /// <summary>
        ///     addRule method adds a production rule and variable to L-System
        /// </summary>
        /// <param name="rule"></param>
        public void addRule(IRule rule)
        {
            rules.Add(rule.var, rule);
        }

        /// <summary>
        ///     method updates current sentence to the next sentence using product rules
        /// </summary>
        public void next()
        {
            Sentence tmp;
            lastSentence.Clear();

            Sentence.Enumerator enumerator = sentence.GetEnumerator() as Sentence.Enumerator;

            while(enumerator.MoveNext())
            {
                if(this.rules.ContainsKey(enumerator.Current))
                {
                    this.rules[enumerator.Current].replace(enumerator, lastSentence);
                }
                else
                {
                    lastSentence.Add(enumerator.Current);
                }
            }

            tmp = sentence;

            sentence = lastSentence;
            lastSentence = tmp;
        }

        /// <summary>
        ///     Method updates current sentence to the sentence after n recursive runs
        /// </summary>
        public void next(int count)
        {
            for (int i1 = 0; i1 < count; i1++)
            {
                next();
            }
        }

        /// <summary>
        ///     Update current sentence into intial axiom
        /// </summary>
        public void restart()
        {
            lastSentence.Clear();
            sentence.Clear();
            sentence.InsertSentence(0, axiom.Clone());
        }
    }
}
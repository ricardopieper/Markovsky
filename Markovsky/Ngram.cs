using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Markovsky
{
    public class Ngram
    {
        public String[] Words { get; set; }

        public override bool Equals(object obj)
        {
            var ngram = obj as Ngram;
            return ngram != null &&
                   Enumerable.SequenceEqual(this.Words, ngram.Words);
        }

        public override int GetHashCode()
        {
            int hash = 1573584826;
            foreach (var w in Words)
            {
                hash += 23 * w.GetHashCode();
            }
            return hash;
        }

        public static bool operator ==(Ngram ngram1, Ngram ngram2)
        {
            return EqualityComparer<Ngram>.Default.Equals(ngram1, ngram2);
        }

        public static bool operator !=(Ngram ngram1, Ngram ngram2)
        {
            return !(ngram1 == ngram2);
        }

        public override string ToString()
        {
            return String.Join(" ", Words);
        }
    }
}

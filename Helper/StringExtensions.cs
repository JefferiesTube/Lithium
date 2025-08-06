using System.Text;

namespace Lithium.Helper
{
    public static class StringExtensions
    {
        public static string SmartJoin<T>(this IEnumerable<T> source, string glue, string lastGlue)
        {
            T[] array = source.ToArray();

            switch (array.Length)
            {
                case 0:
                    return string.Empty;
                case 1:
                    return array[0].ToString();
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(string.Join(glue, array[..^1]));
            sb.Append($"{lastGlue}{array.Last()}");
            return sb.ToString();
        }
    }
}

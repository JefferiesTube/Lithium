namespace Lithium.Helper
{
    public static class CollectionConverter
    {
        public static T[] ToArray<T>(this Il2CppSystem.Collections.Generic.List<T> list)
        {
            if (list == null)
                return [];
            T[] array = new T[list.Count];
            int c = 0;
            foreach (T item in list)
            {
                array[c] = item;
                c++;
            }
            return array;
        }

        public static List<T> ToList<T>(this Il2CppSystem.Collections.Generic.List<T> list)
        {
            if (list == null)
                return [];
            List<T> result = new List<T>(list.Count);
            foreach (T item in list)
            {
                result.Add(item);
            }
            return result;
        }
    }
}

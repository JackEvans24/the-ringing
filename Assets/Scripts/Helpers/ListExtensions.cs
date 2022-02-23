using System.Collections.Generic;

namespace Assets.Scripts.Helpers
{
    public static class ListExtensions
    {
        /// <summary>
        /// Moves an item to the specified index, retaining the order of other items
        /// </summary>
        public static void Move<T>(this List<T> list, T item, int newIndex)
        {
            if (item != null)
            {
                var oldIndex = list.IndexOf(item);
                if (oldIndex < 0)
                    return;

                while (oldIndex != newIndex)
                {
                    var oldItem = list[newIndex];

                    list[newIndex] = item;
                    list[oldIndex] = oldItem;

                    item = oldItem;
                    newIndex += newIndex > oldIndex ? -1 : 1;
                }
            }
        }

        /// <summary>
        /// Swaps an item with the item after it in the list
        /// </summary>
        public static void MoveNext<T>(this List<T> list, T item)
        {
            if (item != null)
            {
                var oldIndex = list.IndexOf(item);
                if (oldIndex < 0)
                    return;

                list.Move(item, oldIndex + 1);
            }
        }

        /// <summary>
        /// Swaps an item with the item before it in the list
        /// </summary>
        public static void MovePrevious<T>(this List<T> list, T item)
        {
            if (item != null)
            {
                var oldIndex = list.IndexOf(item);
                if (oldIndex < 0)
                    return;

                list.Move(item, oldIndex - 1);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace CSET_TA_Assistant
{
    public static class Utility
    {
        public static IEnumerable<Visual> GetChildren(this Visual parent, bool recurse = true)
        {
            if (parent != null)
            {
                int count = VisualTreeHelper.GetChildrenCount(parent);
                for (int i = 0; i < count; i++)
                {
                    // Retrieve child visual at specified index value.
                    var child = VisualTreeHelper.GetChild(parent, i) as Visual;

                    if (child != null)
                    {
                        yield return child;

                        if (recurse)
                        {
                            foreach (var grandChild in child.GetChildren(true))
                            {
                                yield return grandChild;
                            }
                        }
                    }
                }
            }
        }
    }
}

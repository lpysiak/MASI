using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace Uniterm
{
    [Serializable]
    public class MyCanvas : Canvas
    {

        List<Visual> objects = new List<Visual>();

        public void AddElement(Visual v)
        {
            objects.Add(v);
            base.AddVisualChild(v);
            base.AddLogicalChild(v);
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return objects.Count;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            return objects[index];
        }

        public void ClearAll()
        {
            do
            {
                try
                {
                    foreach (Visual v in objects)
                    {
                        objects.Remove(v);
                        base.RemoveLogicalChild(v);
                        base.RemoveVisualChild(v);
                    }
                }
                catch { };
            } while (objects.Count > 0);

        }

        public List<string> GetXAML()
        {
            List<string> obj = new List<string>();
            foreach (object v in base.Children)
            {
                obj.Add(XamlWriter.Save(v));
            }

            return obj;
        }


    }
}

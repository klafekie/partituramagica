using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

//using System.Windows;

namespace PartituraMagica
{
    internal class FakeDrawingPanel : IDrawingPanel
    {
        public string ImagePath;
        public IDictionary<int, string> Labels;
        public int LastId;
        public IDictionary<int, Rect> Rectangles;
        public IDictionary<int, bool> Selection;

        public FakeDrawingPanel()
        {
            Clear();
        }

        public Rect LastRect
        {
            get { return Rectangles[LastId]; }
        }

        public void AddRectangle(int id, Rect coordinates)
        {
            Rectangles.Add(id, coordinates);
            Selection.Add(id, false);
            Labels.Add(id, null);
            LastId = id;
        }

        public void AddOrUpdateRectangle(int id, Rect coordinates)
        {
            if (Rectangles.ContainsKey(id))
            {
                Rectangles[id] = coordinates;
                LastId = id;
            }
            else
            {
                AddRectangle(id, coordinates);
            }
        }

        public void RemoveRectangle(int id)
        {
            Rectangles.Remove(id);
            Selection.Remove(id);
            Labels.Remove(id);
            LastId = -1;
        }

        public void SelectRectangle(int id)
        {
            Selection[id] = true;
        }

        public void DeselectRectangle(int id)
        {
            Selection[id] = false;
        }

        public void Clear()
        {
            Rectangles = new Dictionary<int, Rect>();
            Selection = new Dictionary<int, bool>();
            Labels = new Dictionary<int, string>();
            ImagePath = null;
            LastId = -1;
        }

        public Point DrawImage(string path)
        {
            ImagePath = path;
            return new Point(100, 100);
        }


        public void LabelRectangle(int id, string label)
        {
            Labels[id] = label;
        }


        public IGreyImage CurrentImage
        {
            get { return new FakeImage(); }
        }

        public IGreyImage GetImage(string path)
        {
            return new FakeImage();
        }

        public bool IsSelected(Rect theirs)
        {
            foreach (KeyValuePair<int, Rect> entry in Rectangles)
            {
                if (theirs == entry.Value)
                {
                    return Selection[entry.Key];
                }
            }
            Debug.Assert(false, "Rectangle not present at all");
            return false;
        }
    }
}
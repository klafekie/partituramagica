using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

//using System.Windows;

namespace PartituraMagica
{
    internal class FakeDrawingPanel : IDrawingPanel
    {
        public string imagePath;
        public IDictionary<int, string> labels;
        public int lastId;
        public IDictionary<int, Rect> rectangles;
        public IDictionary<int, bool> selection;

        public FakeDrawingPanel()
        {
            Clear();
        }

        public Rect LastRect
        {
            get { return rectangles[lastId]; }
        }

        public void AddRectangle(int id, Rect coordinates)
        {
            rectangles.Add(id, coordinates);
            selection.Add(id, false);
            labels.Add(id, null);
            lastId = id;
        }

        public void AddOrUpdateRectangle(int id, Rect coordinates)
        {
            if (rectangles.ContainsKey(id))
            {
                rectangles[id] = coordinates;
                lastId = id;
            }
            else
            {
                AddRectangle(id, coordinates);
            }
        }

        public void RemoveRectangle(int id)
        {
            rectangles.Remove(id);
            selection.Remove(id);
            labels.Remove(id);
            lastId = -1;
        }

        public void SelectRectangle(int id)
        {
            selection[id] = true;
        }

        public void DeselectRectangle(int id)
        {
            selection[id] = false;
        }

        public void Clear()
        {
            rectangles = new Dictionary<int, Rect>();
            selection = new Dictionary<int, bool>();
            labels = new Dictionary<int, string>();
            imagePath = null;
            lastId = -1;
        }

        public Point DrawImage(string path)
        {
            imagePath = path;
            return new Point(100, 100);
        }


        public void LabelRectangle(int id, string label)
        {
            labels[id] = label;
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
            foreach (KeyValuePair<int, Rect> entry in rectangles)
            {
                if (theirs == entry.Value)
                {
                    return selection[entry.Key];
                }
            }
            Debug.Assert(false, "Rectangle not present at all");
            return false;
        }
    }
}
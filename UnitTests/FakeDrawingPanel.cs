using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Windows;
using System.Diagnostics;
using System.Windows;

namespace PartituraMagica
{
    class FakeDrawingPanel : IDrawingPanel
    {
        public IDictionary<int, Rect> rectangles;
        public IDictionary<int, bool> selection;
        public IDictionary<int, string> labels;
        public int lastId;
        public string imagePath;

        public FakeDrawingPanel()
        {
            Clear();
        }

        public void AddRectangle(int id, Rect coordinates)
        {
            rectangles.Add(id, coordinates);
            selection.Add(id, false);
            labels.Add(id, null);
            this.lastId = id;
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

        public bool IsSelected(Rect theirs)
        {
            foreach (KeyValuePair<int, Rect> entry in this.rectangles)
            {
                if (theirs == entry.Value)
                {
                    return this.selection[entry.Key];
                }
            }
            Debug.Assert(false, "Rectangle not present at all");
            return false;
        }

        public Rect LastRect
        {
            get { return this.rectangles[this.lastId]; }
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoSpinMVVM.BPMExecution.Controller
{
    public class ShapeDetails
    {
        public string Id { get; set; }

        public string ShapeType { get; set; }

        public List<ShapeDetails> lstWayPoints { get; set; }

        public double Left { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public double Top { get; set; }

        public string ShapeName { get; set; }

        public string Text { get; set; }

        public double LabelLeft { get; set; }

        public double LabelWidth { get; set; }

        public double LabelHeight { get; set; }

        public double LabelTop { get; set; }

        public bool IsExecuted { get; set; }

        public bool IsCurrentShape { get; set; }

        public MapExecutionDetails CallActivityDetails { get; set; }

        public ShapeDetails()
        {
            this.lstWayPoints = new List<ShapeDetails>();
        }
    }
}
﻿using Scumle.Model;
using Scumle.Model.Shapes;
using Scumle.ViewModel;
using Scumle.ViewModel.Shapes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scumle.View.Preview
{
    public static class ShapesPreview
    {

        public static ObservableCollection<IShapeViewModel> List = new ObservableCollection<IShapeViewModel>()
        {
            new BasicShapeViewModel(new BasicShape(EBasicShape.Ellipse, 50, 50)),
            new UMLClassViewModel(new UMLClass(0, 0, "UML Class")),
            new BasicShapeViewModel(new BasicShape(EBasicShape.Rectangle, 0, 0))
        };

        public static ObservableCollection<LineViewModel> LinePreview = new ObservableCollection<LineViewModel>()
        {

        };
    }
}

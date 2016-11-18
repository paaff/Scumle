﻿using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using Scumle.Model;
using Scumle.Model.Shapes;
using Scumle.UndeRedo;
using Scumle.UndeRedo.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Scumle.ViewModel
{
    class MainViewModel : ViewModelBase<Model.Scumle>, INotifyPropertyChanged
    {
        #region Fields
        private int _num = 0;
        private double _zoom = 1.0;
        private bool _isAddingLine;
        private bool _isAddingShape;
        private Cursor _cursor = System.Windows.Input.Cursors.Arrow;
        private bool _isMouseDownOnGrid;
        #endregion

        #region Properties
        public double SelectionX { get; set; }
        public double SelectionY { get; set; }
        public double SelectionWidth { get; set; }
        public double SelectionHeight { get; set; }
        private Point StartingPoint { get; set; }
        public Cursor Cursor
        {
            get { return _cursor; }
            set { _cursor = value; }
        }
        public double Zoom
        {
            get { return _zoom; }
            set
            {
                _zoom = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ShapeViewModel> Shapes { get; }

        public List<ShapeViewModel> Selected { get; } = new List<ShapeViewModel>();

        public ObservableCollection<LineViewModel> Lines { get; } = new ObservableCollection<LineViewModel>();

        public String Version { get; } = "Version 1.0.0";
        #endregion

        #region Commands

        public RelayCommand<string> ChangeZoomCommand { get; set; }
        public RelayCommand SetShapeSelectionCommand { get; }
        public RelayCommand SaveWorkspaceCommand { get; }
        public RelayCommand OpenWorkspaceCommand { get; }
        public RelayCommand DeleteSelectedShapesCommand { get; }
        public RelayCommand NewWorkspaceCommand { get; }
        public RelayCommand UndoCommand { get; }
        public RelayCommand RedoCommand { get; }
        public RelayCommand<MouseButtonEventArgs> MouseDownGridCommand { get; }
        public RelayCommand<MouseEventArgs> MouseMoveGridCommand { get; }
        public RelayCommand<MouseButtonEventArgs> MouseUpGridCommand { get; }
        public RelayCommand EscCommand { get; }
        public UndoRedoController UndoRedo = UndoRedoController.Instance;

        #endregion

        #region Constructor
        public MainViewModel(Model.Scumle scumle) : base(scumle)
        {
            ShapeViewModel uml1 = new UMLClassViewModel(new UMLClass(200, 200, "My frist shape"));
            ShapeViewModel uml2 = new UMLClassViewModel(new UMLClass(50, 50, "My second shape"));
            Shapes = new ObservableCollection<ShapeViewModel>() { uml1, uml2 };

            ConnectionPointViewModel cp1 = uml1.ConnectionPoints.ElementAt(0);
            ConnectionPointViewModel cp2 = uml2.ConnectionPoints.ElementAt(1);

            Lines.Add(new LineViewModel(cp1, cp2));

            EscCommand = new RelayCommand(Escape);
            MouseDownGridCommand = new RelayCommand<MouseButtonEventArgs>(GridMouseDown);
            MouseMoveGridCommand = new RelayCommand<MouseEventArgs>(GridMouseMove);
            MouseUpGridCommand = new RelayCommand<MouseButtonEventArgs>(GridMouseUp);
            SetShapeSelectionCommand = new RelayCommand(SetShapeInsertion);
            ChangeZoomCommand = new RelayCommand<string>(ChangeZoom);
            SaveWorkspaceCommand = new RelayCommand(SaveWorkspace);
            OpenWorkspaceCommand = new RelayCommand(OpenWorkspace);
            NewWorkspaceCommand = new RelayCommand(NewWorkspace);
            DeleteSelectedShapesCommand = new RelayCommand(DeleteSelectedShapes, HasSelectedShapes);
            RedoCommand = UndoRedoController.Instance.RedoCommand;
            UndoCommand = UndoRedoController.Instance.UndoCommand;
        }

        
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region PropetyChanged

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region selection
        internal void SelectShape(ShapeViewModel shape, bool clearSelection)
        {
            if (clearSelection)
            {
                DeselectAllShapes();
                Selected.Add(shape);
                shape.IsSelected = true;
                DeleteSelectedShapesCommand.RaiseCanExecuteChanged();
            } else {
                Selected.Add(shape);
                shape.IsSelected = true;
                DeleteSelectedShapesCommand.RaiseCanExecuteChanged();
            }
        }

        internal void DeselectAllShapes()
        {
            foreach (ShapeViewModel shape in Selected)
            {
                shape.IsSelected = false;
            }
            Selected.Clear();
            DeleteSelectedShapesCommand.RaiseCanExecuteChanged();
        }
        public bool HasSelectedShapes()
        {
            return Selected.Count > 0;
        }
        #endregion

        #region GridMouseEventHandling
        public void GridMouseDown(MouseButtonEventArgs e)
        {
            if(!_isAddingShape && !_isAddingLine)
            {
                DeselectAllShapes();
                _isMouseDownOnGrid = true;
                e.MouseDevice.Target.CaptureMouse();
                StartingPoint = e.MouseDevice.GetPosition(e.Source as IInputElement);
            }           
            if (_isAddingShape)
            {
                AddShape(StartingPoint);
            }
        }
        public void GridMouseMove(MouseEventArgs e)
        {
            Point curPos = e.MouseDevice.GetPosition(e.Source as IInputElement);
            if (_isMouseDownOnGrid)
            {
                SelectionX = Math.Min(StartingPoint.X, curPos.X);
                SelectionY = Math.Min(StartingPoint.Y, curPos.Y);
                SelectionHeight = Math.Abs(curPos.Y - StartingPoint.Y);
                SelectionWidth = Math.Abs(curPos.X - StartingPoint.X);
                RaisePropertyChanged("SelectionX");
                RaisePropertyChanged("SelectionY");
                RaisePropertyChanged("SelectionHeight");
                RaisePropertyChanged("SelectionWidth");
            }

        }
        public void GridMouseUp(MouseButtonEventArgs e)
        {
            if (_isMouseDownOnGrid)
            {
                Point endingPoint = e.MouseDevice.GetPosition(e.Source as IInputElement);
                double minX = Math.Min(StartingPoint.X, endingPoint.X);
                double minY = Math.Min(StartingPoint.Y, endingPoint.Y);
                double maxX = Math.Max(StartingPoint.X, endingPoint.X);
                double maxY = Math.Max(StartingPoint.Y, endingPoint.Y);
                foreach (ShapeViewModel i in Shapes)
                {
                    if (!(i.X > maxX || i.X + i.Width < minX || i.Y > maxY || i.Y + i.Height < minY))
                    {
                        SelectShape(i, false);
                    }
                }
                _isMouseDownOnGrid = false;
                e.MouseDevice.Target.ReleaseMouseCapture();
                SelectionX = 0;
                SelectionY = 0;
                SelectionHeight = 0;
                SelectionWidth = 0;
                RaisePropertyChanged("SelectionX");
                RaisePropertyChanged("SelectionY");
                RaisePropertyChanged("SelectionHeight");
                RaisePropertyChanged("SelectionWidth");
            }
        }

        #endregion

        #region Methods

        private void Escape()
        {
            DeselectAllShapes();
        }

        public void Undo()
        {
            UndoRedoController.Instance.Undo();
            UndoCommand.RaiseCanExecuteChanged();
            RedoCommand.RaiseCanExecuteChanged();
        }

        public void Redo()
        {
            UndoRedoController.Instance.Redo();
            UndoCommand.RaiseCanExecuteChanged();
            RedoCommand.RaiseCanExecuteChanged();
        }

        public void SetShapeInsertion()
        {
            _isAddingShape = true;
            _cursor = System.Windows.Input.Cursors.Cross;
            RaisePropertyChanged("Cursor");
        }

        public void AddShape(Point p)
        {        
                ShapeViewModel shape = new UMLClassViewModel(new Eclipse(p.X, p.Y, "My shape " + _num++));
                new ShapeAddCommand(Shapes, shape).Execute();
                _isAddingShape = false;
                _cursor = System.Windows.Input.Cursors.Arrow;
                RaisePropertyChanged("Cursor");
        }

        public void ChangeZoom(string value)
        {
            Zoom = Double.Parse(value);
        }

        public void DeleteSelectedShapes()
        {
            new ShapeRemoveCommand(Shapes, Lines, Selected).Execute();
            DeselectAllShapes();
        }

        public void DeleteShape(ShapeViewModel shape)
        {
            shape.IsSelected = false;
            Shapes.Remove(shape);
            RemoveLines(shape);
        }

        public void RemoveLines(ShapeViewModel shape)
        {
            foreach (LineViewModel line in Lines.ToList())
            {
                if (line.From.Shape == shape || line.To.Shape == shape)
                {
                    Lines.Remove(line);
                }
            }
        }

        public void SaveWorkspace()
        {
            SaveFileDialog save = new SaveFileDialog();
            save.DefaultExt = ".scumle";
            save.Filter = "(.scumle)|*.scumle";
            if (save.ShowDialog() == true)
            {
                List<Shape> modelsToSave = new List<Shape>();

                foreach (var ViewModel in Shapes)
                {
                    modelsToSave.Add(ViewModel.Model);
                }

                Helpers.GenericSerializer.convertToXML(modelsToSave, Path.GetFullPath(save.FileName));
            }


        }

        public void OpenWorkspace()
        {

            OpenFileDialog open = new OpenFileDialog();
            open.DefaultExt = ".scumle";
            open.Filter = "(.scumle)|*.scumle";
            List<Shape> loadedModels = new List<Shape>();



            // Show open file dialog box
            Nullable<bool> result = open.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                loadedModels = Helpers.GenericSerializer.convertFromXML<List<Shape>>(Path.GetFullPath(open.FileName));
                Shapes.Clear();
                foreach (var loadedModel in loadedModels)
                {
                    if (loadedModel is UMLClass)
                    {
                        Shapes.Add(new UMLClassViewModel(loadedModel));
                    }
                    else
                    {
                        Shapes.Add(new ShapeViewModel(loadedModel));
                    }
                }

            }
        }

        //TODO: Implement adding a new "window pane" instead of just deleting the one we have.
        public void NewWorkspace()
        {
            Shapes.Clear();
        }

        #endregion

    }

}

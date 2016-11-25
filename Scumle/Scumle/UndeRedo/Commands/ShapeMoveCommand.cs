﻿using Scumle.Model;
using Scumle.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Scumle.UndeRedo.Commands
{
    class ShapeMoveCommand : UndoRedoCommand
    {
        IShape shape;
        Point oldPos;
        Point newPos;

        public ShapeMoveCommand(IShape _shape, Point _oldPos, Point _newPos)
        {
            shape = _shape;
            oldPos = _oldPos;
            newPos = _newPos;
        }

        public override void Redo()
        {
            shape.X = newPos.X;
            shape.Y = newPos.Y;
        }

        public override void Undo()
        {
            shape.X = oldPos.X;
            shape.Y = oldPos.Y;
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scumle.Model;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Scumle.Model.Shapes;
using System.Windows;

namespace Scumle.ViewModel
{
    public class UMLClassViewModel : ShapeViewModel
    {

        public ObservableCollection<ListItem> fields;
        public ObservableCollection<ListItem> methods;

        public ICommand removeFieldCommand => new RelayCommand<ListItem>(removeField);
        public ICommand removeMethodCommand => new RelayCommand<ListItem>(removeMethod);
        public ICommand addFieldCommand => new RelayCommand(addField);
        public ICommand addMethodCommand => new RelayCommand(addMethod);

        #region Constructor
        // This Constructor should be taking an UMLClass and not a Shape yes?
        public UMLClassViewModel(UMLClass uml) : base(uml)
        {
            Width = 300;
            Height = 150;
            ShapeColor = new SolidColorBrush(Color.FromRgb(232, 232, 232));

          //  fields = new ObservableCollection<ListItem>(e => );
          //  methods = new ObservableCollection<ListItem>(uml.umlMethods);


        }
        #endregion

        #region Properties

        
      
        public ObservableCollection<ListItem> UMLFields
        {
            get { return fields; }
            set { }
        }

        public ObservableCollection<ListItem> UMLMethods
        {
            get { return methods; }
            set { }
        }
        #endregion

        private void removeField(ListItem field)
        {
            UMLFields.Remove(field);
           
        }
        
        private void removeMethod(ListItem method)
        {
            UMLMethods.Remove(method);
            
        }

        private void addField()
        {
            UMLFields.Add(new ListItem { ListValue = "New Field .. " });
        }

        private void addMethod()
        {
            UMLMethods.Add(new ListItem { ListValue = "New Method .. " });
        }




        #region ListItemClass
        public class ListItem
        {
            public string ListValue { get; set; }
        }
        #endregion

    }
}

﻿#region Copyright (c) 2003-2005, Luke T. Maxon

/********************************************************************************************************************
'
' Copyright (c) 2003-2005, Luke T. Maxon
' All rights reserved.
' 
' Redistribution and use in source and binary forms, with or without modification, are permitted provided
' that the following conditions are met:
' 
' * Redistributions of source code must retain the above copyright notice, this list of conditions and the
' 	following disclaimer.
' 
' * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and
' 	the following disclaimer in the documentation and/or other materials provided with the distribution.
' 
' * Neither the name of the author nor the names of its contributors may be used to endorse or 
' 	promote products derived from this software without specific prior written permission.
' 
' THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED
' WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
' PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
' ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
' LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
' INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
' OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN
' IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
'
'*******************************************************************************************************************/

#endregion

//Contributed by: Ian Cooper

using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using mRemoteNGTests.TestHelpers;

namespace mRemoteNGTests
{
    /// <summary>
    /// A ControlTester for testing List Views.  
    /// </summary>
    /// <remarks>
    /// It includes helper methods for selecting items from the list
    /// and for clearing those selections.</remarks>
    public class ListViewTester
    {
        private readonly string _name;
        private readonly Form _form;

        /// <summary>
        /// Creates a ControlTester from the control name and the form instance.
        /// </summary>
        /// <remarks>
        /// It is best to use the overloaded Constructor that requires just the name 
        /// parameter if possible.
        /// </remarks>
        /// <param name="name">The Control name.</param>
        /// <param name="form">The Form instance.</param>
        public ListViewTester(string name, Form form)
        {
            _name = name;
            _form = form;
        }

        /// <summary>
        /// Provides access to all of the Properties of the ListBox.
        /// </summary>
        /// <remarks>
        /// Allows typed access to all of the properties of the underlying control.
        /// </remarks>
        /// <value>The underlying control.</value>
        public ListView Properties => _form.FindControl<ListView>(_name);

        /// <summary>
        /// Helper method to return the List View's Items property
        /// </summary>
        public ListView.ListViewItemCollection Items => Properties.Items;

        /// <summary>
        /// Helper method to return the columns of the list view
        /// </summary>
        public ListView.ColumnHeaderCollection Columns => Properties.Columns;

        /// <summary>
        /// Clears the selections from the list box.
        /// </summary>
        public void ClearSelected()
        {
            foreach (ListViewItem item in Properties.Items)
            {
                item.Selected = false;
            }
        }

        /// <summary>
        /// Selects an item in the ListBox according to its index.
        /// </summary>
        /// <param name="i">the index to select.</param>
        public void Select(int i)
        {
            Properties.Items[i].Selected = true;
            
            FireEvent("ItemActivate");
        }

        private void FireEvent(string eventName)
        {
            var ctrl = Properties;
            MethodInfo method = typeof(ListView).GetMethod("On" + eventName, BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(ctrl, new object[] { EventArgs.Empty });
        }

        /// <summary>
        /// Selects an item in the list according to its string value.
        /// </summary>
        /// <param name="text">The item to select.</param>
        public void Select(string text)
        {
            int index = FindItemByString(text);

            if (ItemFound(index))
            {
                Select(index);
            }
        }

        /// <summary>
        /// Multiple selection of a range of items
        /// </summary>
        /// <param name="items"></param>
        public void SelectItems(string[] items)
        {
            foreach (string item in items)
            {
                Select(item);
            }
        }

        /// <summary>
        /// Test that only the indicated items are selected
        /// </summary>
        /// <param name="matchList"></param>
        public bool SelectedItemsMatch(string[] matches)
        {
            ArrayList matchList = new(matches);

            if (matchList.Count != Properties.SelectedItems.Count)
            {
                return false;
            }

            foreach (ListViewItem item in Properties.SelectedItems)
            {
                if (!matchList.Contains(item.Text))
                {
                    return false;
                }
            }

            return true;
        }

        #region Implementation

        private bool ItemFound(int index)
        {
            return index != -1;
        }

        private int FindItemByString(string text)
        {
            for (int i = 0; i < Properties.Items.Count; i++)
            {
                if (Properties.Items[i].Text == text)
                {
                    return i;
                }
            }

            return -1;
        }

        #endregion
    }
}
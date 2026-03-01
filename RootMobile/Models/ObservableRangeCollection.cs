using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace RootMobile.Models;

public class ObservableRangeCollection<T> : ObservableCollection<T>
{
   public void AddRange(IEnumerable<T> collection)
   {
      // Add all items to the underlying collection
      foreach (var i in collection)
      {
         Items.Add(i);
      }
 
      // Send a *single* notification to the UI that the collection has changed.
      // The UI will then update itself just once.
      OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
   }
}
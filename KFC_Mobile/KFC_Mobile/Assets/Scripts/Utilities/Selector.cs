using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Selector<T> : ICollection<T>, IList<T> {

    public System.Action<T> Activate;
    public System.Action<T> Deactivate;
    
    private List<T> data = new List<T> ();
    private int cursor = 0;
    
    public void Left() {
        if( data.Count  > 1 )
        {
            Deactivate(data[cursor]);
            cursor = (data.Count + cursor - 1) % data.Count ;
            Activate(data[cursor]);
        }
    }
    
    public void Right() {
        if( data.Count  > 1 )
        {
            Deactivate(data[cursor]);
            cursor = (cursor + 1) % data.Count ;
            Activate(data[cursor]);
        }
    }

    public void Up()
    {
        Left();
    }

    public void Down()
    {
        Right();
    }

    public T GetSelection()
    {
        return data[cursor];
    }

    public void Reset()
    {
        foreach (T item in data)
        {
            Deactivate(item);
        }

        Activate(data[cursor]);
    }

    //This Constructor will let you do this: ShuffleBag<int> intBag = new ShuffleBag<int>(new int[] {1, 2, 3, 4, 5});
    public Selector(System.Action<T> activate, System.Action<T> deactivate, params T[] list)
    {
        for( int i = list.Length - 1; i >= 0; i-- )
        {
            Add(list[i]);
        }

        Activate = activate;
        Deactivate = deactivate;

        Reset();
    }
    
    public Selector(BehaviorTree.Sequence<PlayerMovement> sequence) { } //Constructor with no values

    #region IList[T] implementation
    public int IndexOf( T item ) {
        return data.IndexOf( item );
    }
    
    public void Insert( int index, T item ) {
        cursor = 0;
        data.Insert( index, item );
    }

    public void RemoveAt( int index ) {
        cursor = 0;
        data.RemoveAt( index );
    }

    public T this[int index] {
        get {
            return data[index];
        }
        set {
            data[index] = value;
        }
    }
    #endregion



    #region IEnumerable[T] implementation
    IEnumerator<T> IEnumerable<T>.GetEnumerator() {
        return data.GetEnumerator();
    }
    #endregion

    #region ICollection[T] implementation
    public void Add( T item ) {
        //Debug.Log (item);
        data.Add( item );
        cursor = data.Count - 1;
    }

    public int Count {
        get {
            return data.Count;
        }
    }

    public void Clear() {
        data.Clear();
    }

    public bool Contains( T item ) {
        return data.Contains( item );
    }

    public void CopyTo( T[] array, int arrayIndex ) {
        foreach( T item in data ) {
            array.SetValue( item, arrayIndex );
            arrayIndex = arrayIndex + 1;
        }
    }

    public bool Remove( T item ) {
        cursor = data.Count - 2;
        return data.Remove( item );
    }

    public bool IsReadOnly {
        get {
            return false;
        }
    }
    #endregion

    #region IEnumerable implementation
    IEnumerator IEnumerable.GetEnumerator() {
        return data.GetEnumerator();
    }
    #endregion

}
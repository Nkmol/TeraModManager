using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

public class Mod : INotifyPropertyChanged
{
    // boiler-plate
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChangedEventHandler handler = PropertyChanged;
        if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
    }
    protected bool SetField<T>(ref T field, T value,
    [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }


    private bool _ischecked;
    public bool IsChecked {
        get { return _ischecked; }
        set { SetField(ref _ischecked, value); } 
    }
    public string Name { get; set; }
    public string Author { get; set; }
    public string Type { get; set; }
    public string Date { get; set; }
    public string Website { get; set; }
    public string Version { get; set; }
    public string Code { get; set; }
    public string Path { get; set; }
}

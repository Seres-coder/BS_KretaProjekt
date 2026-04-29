using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace KretaAvalonia_SB.ViewModels;

public abstract class ViewModelBase : INotifyPropertyChanged
{
    protected ViewModelBase() { }

    public event PropertyChangedEventHandler? PropertyChanged;
    //Értesíti a UI-t hogy egy property értéke megváltozott
    protected virtual void OnPropertyChanged([CallerMemberName] String? propertyName = null)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

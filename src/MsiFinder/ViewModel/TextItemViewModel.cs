// Copyright (c) Yaroslav Bugaria. All rights reserved.

using MsiFinder.Model;

namespace MsiFinder.ViewModel;

public class TextItemViewModel : ItemViewModel
{
    private string _text = "Loading...";
    private TextItemType _type;

    public string Text
    {
        get => _text;
        set => Set(ref _text, value);
    }

    public TextItemType Type
    {
        get => _type;
        set => Set(ref _type, value);
    }
}

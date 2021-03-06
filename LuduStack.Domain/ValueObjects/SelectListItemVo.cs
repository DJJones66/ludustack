﻿namespace LuduStack.Domain.ValueObjects
{
    public class SelectListItemVo
    {
        public string Value { get; set; }

        public string Text { get; set; }

        public bool Selected { get; set; }

        public SelectListItemVo()
        {
        }

        public SelectListItemVo(string text, string value)
        {
            Text = text;
            Value = value;
        }
    }

    public class SelectListItemVo<TValue>
    {
        public TValue Value { get; set; }

        public string Text { get; set; }

        public bool Selected { get; set; }
    }
}
﻿using System;
using Android.App;
using Android.Content;
using Android.OS;

namespace Agenda
{

    public class DatePickerFragment : DialogFragment
    {
        private readonly Context _context;
        private DateTime _date;
        private readonly DatePickerDialog.IOnDateSetListener _listener;

        public DatePickerFragment(Context context, DateTime date, Android.App.DatePickerDialog.IOnDateSetListener listener)
        {
            _context = context;
            _date = date;
            _listener = listener;
        }

        public override Dialog OnCreateDialog(Bundle savedState)
        {
            var dialog = new DatePickerDialog(_context, _listener, _date.Year, _date.Month - 1, _date.Day);
            return dialog;
        }

    }

}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;


namespace Essence.Model
{
    public class PostModel: INotifyPropertyChanged
    {
        private int _userId;
        private int _id;
        private string _title;
        private string _body;


        public int userId { get { return _userId; } set { _userId = value; NotifyPropertyChanged("userId"); } }
        public int id { get { return _id; } set { _id = value; NotifyPropertyChanged("id"); } }
        public string title { get { return _title; } set { _title = value; NotifyPropertyChanged("title"); } }
        public string body { get { return _body; } set { _body = value; NotifyPropertyChanged("body"); } }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WpfApp1
{
    class Search
    {


        //Методы SearchNext, SearchPrev перенести сюда


        private static List<int> SearchIndexes;
        /// <summary>
        /// Get list of indexes with searching word
        /// </summary>
        /// <param name="CurrentN">Current number</param>
        /// <param name="SearchTextBox">From what textbox search?</param>
        /// <param name="AccountsList">Where to serach?</param>
        /// <returns></returns>
        public static List<int> GetIndexes(ref int CurrentN, TextBox SearchTextBox, ListBox AccountsList)
        {
            SearchIndexes = new List<int>();
            CurrentN = 0;
            if (SearchTextBox.Text != string.Empty)
            {
                int currentIndex = 0;
                foreach (object item in AccountsList.Items)
                {
                    if (item.ToString().ToLower().Contains(SearchTextBox.Text.ToLower()))
                        SearchIndexes.Add(currentIndex);
                    currentIndex++;
                }
                if (SearchIndexes.Count != 0)
                {
                    AccountsList.SelectedIndex = SearchIndexes[0];
                    AccountsList.ScrollIntoView(AccountsList.SelectedItem);
                    //NextButton.IsEnabled = true;
                    //PrevButton.IsEnabled = true;
                    //label6.Text = string.Format("{0}/{1}", 1, SearchIndexes.Count);
                }
            }
            else
            {
                AccountsList.SelectedIndex = -1;
                //NextButton.IsEnabled = false;
                //PrevButton.IsEnabled = false;
                //label6.Text = "0/0";
            }
            return SearchIndexes;
        }
    }
}

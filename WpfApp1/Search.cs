using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    public class Search
    {
        /// <summary>
        /// Переход к следующему элементу
        /// </summary>
        public void SearchNext()
        {
            if (!isUpdateIndex)
            {
                UpdateIndexes();
            }
            if (SearchIndexes.Count != 0)
            {
                CurrentNumber++;
                if (CurrentNumber < SearchIndexes.Count)
                {
                    AccountsList.SelectedIndex = SearchIndexes[CurrentNumber];
                    AccountsList.ScrollIntoView(AccountsList.SelectedItem);
                }
                else
                {
                    CurrentNumber = 0;
                    AccountsList.SelectedIndex = SearchIndexes[CurrentNumber];
                    AccountsList.ScrollIntoView(AccountsList.SelectedItem);
                }
                SearchTextBox.Focus();
                searchLabel.Text = string.Format("{0}/{1}", CurrentNumber + 1, SearchIndexes.Count);
            }
        }

        /// <summary>
        /// Переход к предыдущему элементу
        /// </summary>
        public void SearchPreview()
        {
            if (!isUpdateIndex)
            {
                UpdateIndexes();
            }
            if (SearchIndexes.Count != 0)
            {
                CurrentNumber--;
                if (CurrentNumber < 0)
                {
                    CurrentNumber = SearchIndexes.Count - 1;
                    AccountsList.SelectedIndex = SearchIndexes[CurrentNumber];
                    AccountsList.ScrollIntoView(AccountsList.SelectedItem);
                }
                else
                {
                    AccountsList.SelectedIndex = SearchIndexes[CurrentNumber];
                    AccountsList.ScrollIntoView(AccountsList.SelectedItem);
                }
                SearchTextBox.Focus();
                searchLabel.Text = string.Format("{0}/{1}", CurrentNumber + 1, SearchIndexes.Count);
            }
        }

        public Search(TextBox stb, ListBox box, TextBlock se)
        {
            SearchTextBox = stb;
            AccountsList = box;
            searchLabel = se;
            SearchIndexes = new List<int>();
            isUpdateIndex = true;
        }

        TextBox SearchTextBox;
        public List<int> SearchIndexes { get; set; }
        ListBox AccountsList;
        TextBlock searchLabel;
        public int CurrentNumber { get; set; }
        public bool isUpdateIndex { get; set; }

        /// <summary>
        /// Получение индексов из AccountList по искомому слову
        /// </summary>
        /// <returns></returns>
        public List<int> UpdateIndexes()
        {
            SearchIndexes = new List<int>();
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
                }
                searchLabel.Text = string.Format("{0}/{1}", SearchIndexes.Count == 0 ? 0 : 1, SearchIndexes.Count);
                isUpdateIndex = true;
            }
            else
            {
                AccountsList.SelectedIndex = -1;
                searchLabel.Text = string.Format("{0}/{1}", SearchIndexes.Count == 0 ? 0 : 1, SearchIndexes.Count);
            }
            return SearchIndexes;
        }
    }
}

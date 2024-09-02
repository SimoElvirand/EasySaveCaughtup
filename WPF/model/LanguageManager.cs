using System;
using System.Windows;

namespace WPF.model
{
    public class LanguageManager
    {
        public event EventHandler LanguageChanged;

        private static LanguageManager _instance;
        private ResourceDictionary _resourceDictionary;

        private LanguageManager()
        {
            // Set the default language
            SetLanguage("en");
        }

        public static LanguageManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LanguageManager();
                }
                return _instance;
            }
        }

        public void SetLanguage(string languageCode)
        {
            _resourceDictionary = new ResourceDictionary();

            switch (languageCode)
            {
                case "es":
                    _resourceDictionary.Source = new Uri("utils/ressources/ESDictionary.xaml", UriKind.Relative);
                    break;
                case "ar":
                    _resourceDictionary.Source = new Uri("utils/ressources/ARDictionary.xaml", UriKind.Relative);
                    break;
                case "ch":
                    _resourceDictionary.Source = new Uri("utils/ressources/CHDictionary.xaml", UriKind.Relative);
                    break;
                case "en":
                    _resourceDictionary.Source = new Uri("utils/ressources/ENDictionary.xaml", UriKind.Relative);
                    break;
                case "fr":
                    _resourceDictionary.Source = new Uri("utils/ressources/FRDictionary.xaml", UriKind.Relative);
                    break;
                default:
                    _resourceDictionary.Source = new Uri("utils/ressources/ENDictionary.xaml", UriKind.Relative);
                    break;
            }

            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(_resourceDictionary);

            // Notify subscribers (views) that the language has changed
            OnLanguageChanged();
        }

        protected virtual void OnLanguageChanged()
        {
            LanguageChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}

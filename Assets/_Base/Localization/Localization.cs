using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using UnityEngine;

namespace BaseFramework
{
    public class Localization : Singleton<Localization>
    {
    
// https://stackoverflow.com/questions/8493195/how-can-i-parse-a-csv-string-with-javascript-which-contains-comma-in-data/8497474#8497474
// Validate a CSV string having single, double or un-quoted values.
//     ^                                       # Anchor to start of string.
//     \s*                                     # Allow whitespace before value.
//     (?:                                     # Group for value alternatives.
//     '[^'\\]*(?:\\[\S\s][^'\\]*)*'           # Either Single quoted string,
//     | "[^"\\]*(?:\\[\S\s][^"\\]*)*"         # or Double quoted string,
//     | [^,'"\s\\]*(?:\s+[^,'"\s\\]+)*        # or Non-comma, non-quote stuff.
//     )                                       # End group of value alternatives.
//     \s*                                     # Allow whitespace after value.
//     (?:                                     # Zero or more additional values
// ,                                           # Values separated by a comma.
//     \s*                                     # Allow whitespace before value.
//     (?:                                     # Group for value alternatives.
//     '[^'\\]*(?:\\[\S\s][^'\\]*)*'           # Either Single quoted string,
//     | "[^"\\]*(?:\\[\S\s][^"\\]*)*"         # or Double quoted string,
//     | [^,'"\s\\]*(?:\s+[^,'"\s\\]+)*        # or Non-comma, non-quote stuff.
//     )                                       # End group of value alternatives.
//     \s*                                     # Allow whitespace after value.
//     )*                                      # Zero or more additional values
//     $                   
        
        private List<string> languages;
        private Dictionary<string, string[]> localizationText;
        
        private string defaultLanguage { get; set; }

        /// <summary>
        /// 传入csv字符串
        /// </summary>
        /// <param name="textConfig"></param>
        public void Init(string textConfig, string lang)
        {
            defaultLanguage = lang;
            
            // split line
            string[] lines = textConfig.Split(new char[]{'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries);
                
            if(lines.IsNullOrEmpty())
                return;
            
            // Parse CVS line. Capture next value in named group: 'val'
            Regex pattern = new Regex(@"\s*(?:""(?<val>[^""]*(""""[^""]*)*)""\s*|(?<val>[^,]*))(?:,|$)", 
                                      RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
            
            try
            {
                lines.ForEach((index, line) =>
                              {
                                  MatchCollection matchCollection = pattern.Matches(line);
                                  List<string> words = new List<string>();
                                  for (int i = 0; i < matchCollection.Count; ++i)
                                  {
                                      string word = matchCollection[i].Groups["val"].Value;
                                      if(word.Any())
                                          words.Add(word);
                                  }
                                  // 首行是语言标题栏
                                  if (index == 0)
                                  {
                                      if (words.Count < 2)
                                          throw new ArgumentException("localization language not define!");

                                      languages = new List<string>();
                                      // 首行第一列不解析
                                      for (int i = 1; i < words.Count; ++i)
                                      {
                                            languages.Add(words[i].ToLower());
                                      }
                                  }
                                  // 非首行
                                  else
                                  {
                                      if(words.Count < languages.Count + 1)
                                          throw new ArgumentException($"localization config text error at line:{index + 1}");
                                      
                                      if (localizationText == null)
                                      {
                                          localizationText = new Dictionary<string, string[]>();
                                      }

                                      string key = words[0];
                                      if (localizationText.ContainsKey(key))
                                      {
                                          throw new ArgumentException($"duplicate key:{key}");
                                      }

                                      localizationText[key] = new string[languages.Count];
                                      for (int i = 1; i < words.Count; ++i)
                                      {
                                          localizationText[key][i - 1] = words[i];
                                      }
                                  }
                              });             
            } 
            catch (ArgumentException ex) {
                Log.E(this, $"config parse error! {ex.Message}");
            }
        }

        public string GetText(string key, string language = null, string defaultValue = null)
        {
            if (language == null)
            {
                language = defaultLanguage;
            }
            
            if (languages == null
                || localizationText == null
                || language.IsNullOrEmpty()
                || key.IsNullOrEmpty()
                || !localizationText.ContainsKey(key))
                return defaultValue;

            int index = languages.FindIndex(it => it == language.ToLower());
            if (index < 0 || index >= localizationText[key].Length)
                return defaultValue;

            return localizationText[key][index];
        }
    }
}
using mymemo.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace mymemo.ViewModel
{


    /// <summary>
    /// ViewModel
    /// </summary>
    class ViewModel : ViewModelBase
    {
        string filePath = string.Empty;
        bool bRemove = false;
        int selectedIndex = -1;

        // コンストラクタ
        public ViewModel()
        {
            filePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\memo.txt";

            taskList = new ObservableCollection<MainModel>();
            addCommand = new MainCommand(ExecuteAdd);
            remCommand = new MainCommand(ExecuteRemove);
            doneCommand = new MainCommand(ExecuteDone);

            updatedate = DateTime.Today;

            ExecuteLoad();

            Application.Current.Exit += Current_Exit;

            
        }

        private void Current_Exit(object sender, ExitEventArgs e)
        {
            ExecuteDone();
        }

        #region プロパティ
        private ObservableCollection<MainModel> taskList;
        public ObservableCollection<MainModel> TaskList
        {
            get { return taskList; }
            set
            {
                taskList = value;
                RaisePropertyChanged("TaskList");
            }
        }
        private MainCommand addCommand;
        public MainCommand AddCommand
        {
            get { return addCommand; }
        }
        private MainCommand doneCommand;
        public MainCommand DoneCommand
        {
            get { return doneCommand; }
        }
        private MainCommand remCommand;
        public MainCommand RemCommand
        {
            get { return remCommand; }
        }

        private ICommand selectedCommand;
        public ICommand SelectedCommand
        {
            get
            {
                if (selectedCommand == null)
                {
                    selectedCommand = new RelayCommand(ExecuteSelectCommand);
                }
                return selectedCommand;
            }
        }

        private int selectListIndex = -1;

        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                RaisePropertyChanged("Title");
            }
        }
        private DateTime updatedate;
        public DateTime Updatedate
        {
            get { return updatedate; }
            set
            {
                updatedate = value;
                RaisePropertyChanged("Updatedate");
            }
        }
        private string userid;
        public string UserID
        {
            get { return userid; }
            set
            {
                userid = value;
                RaisePropertyChanged("UserID");
            }
        }
        private string password;
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                RaisePropertyChanged("Password");
            }
        }

        private string memo;
        public string Memo
        {
            get { return memo; }
            set
            {
                memo = value;
                RaisePropertyChanged("Memo");
            }
        }


        #endregion

        private void textboxClear()
        {
            Title = string.Empty;
            UserID = string.Empty;
            Password = string.Empty;
            Memo = string.Empty;
        }


        /// <summary>
        /// 追加処理
        /// </summary>
        private void ExecuteAdd()
        {
            if (string.IsNullOrEmpty(title)) { return; }
            //if (selectedIndex > -1) { return; }

            var task = new MainModel();
            task.Title = title;
            task.UserID = userid;
            task.Password = password;
            task.Memo = memo;
            task.UpdateDate = updatedate;
            task.Done = false;

            bool bflg = false;
            for (int i = 0; i < this.taskList.Count; i++)
            {
                if (this.taskList[i].Title.Equals(title))
                {
                    this.TaskList[i] = task;
                    bflg = true;
                    break;
                }
            }

            if (!bflg)
            {
                taskList.Add(task);
            }

            textboxClear();
            //保存
            ExecuteDone();

        }

        /// <summary>
        /// 編集処理
        /// </summary>
        private void ExecuteMod()
        {
            if (selectedIndex<0) { return; }

            taskList[selectedIndex].Title = title;
            taskList[selectedIndex].UserID = userid;
            taskList[selectedIndex].Password = password;
            taskList[selectedIndex].Memo = memo;
            taskList[selectedIndex].UpdateDate = updatedate;
            taskList[selectedIndex].Done = false;

            textboxClear();
            //保存
            ExecuteDone();

        }


        /// <summary>
        /// リスト選択
        /// </summary>
        private void ExecuteSelectCommand(object param)
        {
            if (bRemove) { return; }
            ListBox list = param as ListBox;
            selectedIndex = list.SelectedIndex;
            if (selectedIndex < 0) { selectedIndex= this.selectListIndex; }
            Title = taskList[selectedIndex].Title.ToString();
            UserID = taskList[selectedIndex].UserID.ToString();
            Password = taskList[selectedIndex].Password.ToString();
            Memo = taskList[selectedIndex].Memo.ToString();
            this.selectListIndex = selectedIndex;
        }

        /// <summary>
        /// 削除
        /// </summary>
        private void ExecuteRemove()
        {
            bRemove = true;
            try
            {
                for (int i = taskList.Count - 1; i >= 0; i--)
                {
                    if (taskList[i].Done)
                    {
                        taskList.RemoveAt(i);
                    }
                }
                //保存
                ExecuteDone();
            }
            catch { bRemove = false; }
            bRemove = false;
        }


        /// <summary>
        /// 保存
        /// </summary>
        private void ExecuteDone()
        {
            if (File.Exists(filePath))
            {
                try { File.Delete(filePath); } catch { }
            }
            

            try
            {
                StreamWriter sw = new StreamWriter(filePath, true, Encoding.UTF8);
                for (int i = 0; i < taskList.Count; i++)
                {
                    string buff = taskList[i].Title + "," + taskList[i].UserID + "," + taskList[i].Password 
                            + "," + taskList[i].Memo + "," +  DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")+ System.Environment.NewLine ;
                    sw.Write(buff);
                }
                sw.Close();
            }
            catch { }
        }


        /// <summary>
        /// 読み込み
        /// </summary>
        private void ExecuteLoad()
        {
            
            if (!File.Exists(filePath))
            {
                return;
            }
            string Line = string.Empty;
            ArrayList al = new ArrayList();
            using (StreamReader sr = new StreamReader( filePath, Encoding.UTF8))
            {

                while ((Line = sr.ReadLine()) != null)
                {
                    al.Add(Line);
                }
            }

            foreach (string item in al)
            {
                try
                {
                    string[] items = item.Split(',');
                    this.title = items[0];
                    this.userid = items[1];
                    this.password = items[2];
                    this.memo = items[3];
                    this.ExecuteAdd();
                }
                catch { continue; }
            }
            textboxClear();
        }



        /// <summary>
        /// Command用クラス
        /// </summary>
        public sealed class RelayCommand : ICommand
        {

            #region 内部変数

            /// <summary>実行用メソッド</summary>
            private Action<object> execute_ = null;

            /// <summary>実行可否判定用メソッド</summary>
            private Predicate<object> canExecute_ = null;

            #endregion

            #region コンストラクタ

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="execute">実行用メソッド</param>
            /// <param name="canExecute">実行可否判定用メソッド</param>
            public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
            {
                execute_ = execute;
                canExecute_ = canExecute;
            }

            #endregion

            #region ICommand メンバー

            /// <summary>
            /// 実行可能かどうかを取得します。
            /// </summary>
            /// <param name="parameter">コマンドパラメータ</param>
            /// <returns>true:実行可能</returns>
            public bool CanExecute(object parameter)
            {
                return canExecute_ == null ? true : canExecute_(parameter);
            }

            /// <summary>変更可否判定値変更イベント</summary>
            public event EventHandler CanExecuteChanged;

            /// <summary>
            /// 実行します。
            /// </summary>
            /// <param name="parameter">コマンドパラメータ</param>
            public void Execute(object parameter)
            {
                execute_(parameter);
                return;
            }

            #endregion
        }

    }

}

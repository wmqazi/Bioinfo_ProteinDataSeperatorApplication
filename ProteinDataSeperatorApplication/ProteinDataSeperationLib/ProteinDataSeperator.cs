using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Qazi.Common;
//amklop
namespace Qazi.DataPreprocessing
{
    public class ProteinDataSeperator : IDisposable 
    {
        private DataTable _ProteinDataTable;
        private DataTable _SiteDataTable;
        private string _PIDFieldName;
        private string _SequenceFieldName;
        private string _PositionFieldName;
        
        private WorkerCompletedEventArg SeperationEventArgs;
        private WorkerProgressEventArg SeperationProgressEventArgs;

        public event WorkerCompletedEventHandler SeperationCompleted;
        public event WorkerProgressUpdateEventHandler SeperationProgressUpdate;
        public event WorkerStartedEventHandler SeperationStarted;

        
        public ProteinDataSeperator(DataTable siteDataTable, string pidFieldName, string sequenceFieldName, string positionFieldName)
        {
            _SiteDataTable  = siteDataTable;
            _PIDFieldName = pidFieldName;
            _SequenceFieldName = sequenceFieldName;
            _PositionFieldName = positionFieldName;
        }

        public void Run()
        {
            if (SeperationStarted != null)
                SeperationStarted(this);

            SeperationEventArgs = new WorkerCompletedEventArg();
            SeperationProgressEventArgs = new WorkerProgressEventArg();

            int total = _SiteDataTable.Rows.Count;
            string positionString;
            string pid;
            string sequence;
            int position;
            _ProteinDataTable = new DataTable("ProteinDataTable");
            _ProteinDataTable.Columns.Add("PID");
            _ProteinDataTable.Columns.Add("Sequence");
            _ProteinDataTable.Columns.Add("SequenceLength");
            _ProteinDataTable.Columns.Add("Position");
            DataRow row;
            Dictionary<string, string> proteinSequenceDictionary;
            Dictionary<string, List<int>> proteinPositionDictionary;

            proteinPositionDictionary = new Dictionary<string, List<int>>();
            proteinSequenceDictionary = new Dictionary<string, string>();
            int index;
            if (SeperationProgressUpdate != null)
            {
                SeperationProgressEventArgs.ProgressPercentage = 0;
                SeperationProgressEventArgs.UserState = "Extracting and Grouping Protein's Sequence and Position Related Data";
                SeperationProgressUpdate(this, SeperationProgressEventArgs);
            }
            for ( index = 0; index < total; index++)
            {
                row = _SiteDataTable.Rows[index];
                pid = row[_PIDFieldName].ToString();
                if (proteinSequenceDictionary.ContainsKey(pid) == false)
                {
                    sequence = row[_SequenceFieldName].ToString();
                    proteinSequenceDictionary.Add(pid, sequence);
                    proteinPositionDictionary.Add(pid, new List<int>());
                }

                position = int.Parse(row[_PositionFieldName].ToString());
                //if (proteinPositionDictionary[pid] == null)
                  //  proteinPositionDictionary[pid] = new List<int>();

                if (proteinPositionDictionary[pid].Contains(position) == false)
                    proteinPositionDictionary[pid].Add(position);

                if (SeperationProgressUpdate != null)
                {

                    SeperationProgressEventArgs.ProgressPercentage = (float)(((float)index / (float)total) * 100.00f);
                    SeperationProgressEventArgs.UserState = "Extracting and Grouping Protein's Sequence and Position Related Data";
                    SeperationProgressUpdate(this, SeperationProgressEventArgs);
                }
            }

            total = proteinPositionDictionary.Keys.Count;
            index = 1;
            int ctr;
            List<int> positionList;

            
            if (SeperationProgressUpdate != null)
            {

                SeperationProgressEventArgs.ProgressPercentage = 0.00f;
                SeperationProgressEventArgs.UserState = "Preparing Protein Profiles and Importing Them to DataTable";
                SeperationProgressUpdate(this, SeperationProgressEventArgs);
            }
            foreach (string id in proteinSequenceDictionary.Keys)
            {
                positionString = "";
                positionList = proteinPositionDictionary[id];
                positionList.Sort();
                for (ctr = 0; ctr < positionList.Count; ctr++)
                {
                    position = positionList[ctr];
                    positionString = positionString + position.ToString();
                    if (ctr != (positionList.Count - 1))
                        positionString = positionString + ",";
                }
                row = _ProteinDataTable.NewRow();
                row["PID"] = id;
                row["Sequence"] = proteinSequenceDictionary[id];
                row["SequenceLength"] = proteinSequenceDictionary[id].Length;
                row["Position"] = positionString;
                _ProteinDataTable.Rows.Add(row);
                if (SeperationProgressUpdate != null)
                {

                    SeperationProgressEventArgs.ProgressPercentage = (float)(((float)index / (float)total) * 100.00f);
                    SeperationProgressEventArgs.UserState = "Preparing Protein Profiles and Importing Them to DataTable";
                    SeperationProgressUpdate(this, SeperationProgressEventArgs);
                }
                index++;

            }

            if (SeperationCompleted != null)
            {
                SeperationCompleted(this, null);
            }
        }


        public void Dispose()
        {
            if (_ProteinDataTable != null)
            {
                _ProteinDataTable.Dispose();
                _ProteinDataTable = null;
            }

            if (_SiteDataTable != null)
            {
                _SiteDataTable.Dispose();
                _SiteDataTable = null;
            }
        }

        public DataTable ProteinDataTable
        {
            get {
                return _ProteinDataTable;
            }
        }

    }
}

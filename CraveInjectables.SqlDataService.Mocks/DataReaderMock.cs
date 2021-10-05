using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace CraveInjectables.SqlDataService.Mocks
{
    public class DataReaderMock<TObject> : IDataReader
    {
        #region Members

        private readonly Dictionary<string, int> _ColumnNameIndexed;
        private object[] _CurrentRow;
        private bool _IsClosed = false;

        private PropertyInfo[] _Properties;

        public int Depth
        {
            get { return 0; }
        }

        public IEnumerator<TObject> Enumerator { get; }

        public bool IsClosed
        {
            get { return _IsClosed; }
        }

        #endregion Members

        #region Constructors

        public DataReaderMock(IEnumerable<TObject> sourceData)
        {
            // We can't use reflection on the TOBject because the user may provide an anonymous object as the values of the sourceData.
            Enumerator = sourceData.GetEnumerator();
            Enumerator.MoveNext();
            _Properties = Enumerator.Current.GetType().GetProperties();
            Enumerator.Reset();

            _ColumnNameIndexed = new Dictionary<string, int>(_Properties.Length);

            for (int i = 0; i < _Properties.Length; i++)
            {
                _ColumnNameIndexed.Add(_Properties[i].Name, i);
            }
        }

        #endregion Constructors

        #region Methods

        private object[] PropertiesToOrderedArray(TObject row)
        {
            var values = new object[_Properties.Length];

            for (int i = 0; i < _Properties.Length; i++)
            {
                values[i] = _Properties[i].GetValue(row);
            }

            return values;
        }

        public int FieldCount
        {
            get { return _Properties.Length; }
        }

        public int RecordsAffected
        {
            get { return -1; }
        }

        public object this[string name]
        {
            get { return GetValue(GetOrdinal(name)); }
        }

        public object this[int i]
        {
            get { return GetValue(i); }
        }

        public void Close()
        {
            _IsClosed = true;
        }

        public void Dispose()
        {
            _IsClosed = true;
            Enumerator.Dispose();
        }

        public bool GetBoolean(int i)
        {
            return (bool)_CurrentRow[i];
        }

        public byte GetByte(int i)
        {
            return (byte)_CurrentRow[i];
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            var binary = (byte[])_CurrentRow[i];
            Array.Copy(binary, fieldOffset, buffer, bufferoffset, length);
            return length;
        }

        public char GetChar(int i)
        {
            return (char)_CurrentRow[i];
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            var binary = (char[])_CurrentRow[i];
            Array.Copy(binary, fieldoffset, buffer, bufferoffset, length);
            return length;
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int i)
        {
            return GetFieldType(i).ToString();
        }

        public DateTime GetDateTime(int i)
        {
            return (DateTime)_CurrentRow[i];
        }

        public decimal GetDecimal(int i)
        {
            return (decimal)_CurrentRow[i];
        }

        public double GetDouble(int i)
        {
            return (double)_CurrentRow[i];
        }

        public Type GetFieldType(int i)
        {
            return _Properties[i].PropertyType;
        }

        public float GetFloat(int i)
        {
            return (float)_CurrentRow[i];
        }

        public Guid GetGuid(int i)
        {
            return (Guid)_CurrentRow[i];
        }

        public short GetInt16(int i)
        {
            return (short)_CurrentRow[i];
        }

        public int GetInt32(int i)
        {
            return (int)_CurrentRow[i];
        }

        public long GetInt64(int i)
        {
            return (long)_CurrentRow[i];
        }

        public string GetName(int i)
        {
            return _Properties[i].Name;
        }

        public int GetOrdinal(string name)
        {
            return _ColumnNameIndexed[name];
        }

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public string GetString(int i)
        {
            return (string)_CurrentRow[i];
        }

        public object GetValue(int i)
        {
            return _CurrentRow[i];
        }

        public int GetValues(object[] values)
        {
            _CurrentRow.CopyTo(values, 0);
            return _CurrentRow.Length;
        }

        public bool IsDBNull(int i)
        {
            return _CurrentRow[i] == null;
        }

        public bool NextResult()
        {
            throw new NotImplementedException("NextResult is not implemented.");
        }

        public bool Read()
        {
            _CurrentRow = null;

            var hasNextRow = Enumerator.MoveNext();

            if (hasNextRow)
                _CurrentRow = PropertiesToOrderedArray(Enumerator.Current);

            return hasNextRow;
        }

        #endregion Methods
    }
}
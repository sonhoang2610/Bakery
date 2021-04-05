/* Native wrap C qhmsg_t */

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace qhmono
{
	public enum QHType{
		QH_UNDEFINED = 0,
		QH_NUMBER = 1,
		QH_STRING = 3,
		QH_VECTOR = 4,
		QH_TABLE = 6
	}

	public abstract class QHObject{

		#if UNITY_IOS
		protected const string dll = "__Internal";
		#elif UNITY_WEBGL && !UNITY_EDITOR
		protected const string dll = "__Internal";
		#else
		protected const string dll = "qhmsg";	
		#endif

		/// Common interface
		[DllImport (dll)]
		protected static extern int qhptr_type (IntPtr qhptr_t);

		[DllImport (dll)]
		protected static extern void qhptr_delete (IntPtr qhptr_t);

		/// Number
		[DllImport(dll)]
		protected static extern IntPtr number_new (long number);

		[DllImport(dll)]
		protected static extern long number_to_int (IntPtr qhptr_t);

		/// String
		// Passing string directly
		[DllImport(dll)]
		protected static extern IntPtr string_new(string str, int size);

		// Passing buffer
		[DllImport(dll)]
		protected static extern IntPtr string_new(byte []buffer, int size);

		// Convert to const char* C string
		[DllImport(dll)]
		protected static extern IntPtr string_to_cstring(IntPtr ptr); 

		// Get length of underlying buffer
		[DllImport(dll)]
		public static extern int string_len(IntPtr ptr); 

		// Get back to raw buffer
		[DllImport(dll)]
		protected static extern uint string_to_buffer(IntPtr ptr, [MarshalAs(UnmanagedType.LPArray)] byte [] buffer, int size); 

		/// Table
		// Constructing new table
		[DllImport(dll)]
		protected extern static IntPtr table_new(uint columns);

		// Delete table object withou concern about it's element
		[DllImport(dll)]
		protected extern static void table_delete(IntPtr table);

		// Return number of columns in table
		[DllImport(dll)]
		protected extern static uint table_column_count(IntPtr table);

		// Get name of a column
		[DllImport(dll)]
		protected extern static IntPtr table_column_get(IntPtr table,uint col);

		// Set name for a column
		[DllImport(dll)]
		protected extern static int table_column_set(IntPtr table,uint col, string name);

		// Return row number
		[DllImport(dll)]
		protected extern static uint table_row_add(IntPtr table);

		// Get an element in table
		[DllImport(dll)]
		protected extern static uint table_row_count(IntPtr table);

		[DllImport(dll)]
		protected extern static int table_row_set(IntPtr table,uint row, uint col,IntPtr ptr);

		[DllImport(dll)]
		protected extern static IntPtr table_row_get(IntPtr table,uint row, uint col);

		/// Vector
		[DllImport(dll)]
		protected extern static IntPtr vector_new();

		//it DON'T delete resource accociate with element
		[DllImport(dll)]
		protected extern static void vector_delete(IntPtr vector);

		//access element by index
		[DllImport(dll)]
		protected extern static IntPtr vector_at(IntPtr vector, int idx);

		//push element to vector
		[DllImport(dll)]
		protected extern static void vector_push(IntPtr vector, IntPtr element);

		//length of vector
		[DllImport(dll)]
		protected extern static int vector_len(IntPtr vector);

		public static QHObject create(QHType type)
		{
			switch (type) {
			case QHType.QH_UNDEFINED:
				return null;
			case QHType.QH_NUMBER:
				return new QHNumber ();
			case QHType.QH_STRING:
				return new QHString ();
			case QHType.QH_TABLE:
				return new QHTable ();
			case QHType.QH_VECTOR:
				return new QHVector();
			}
			return null;
		}

		public static QHObject fromString(string str){
			return new QHString (str);
		}

		public static QHObject fromNumber(long number){
			return new QHNumber (number);
		}

		public QHType getType(){
			return m_Type;
		}

		protected QHType m_Type = QHType.QH_UNDEFINED;


		/// <summary>
		/// Convert IntPtr to QHObject
		/// </summary>
		public static QHObject PtrToQHObject(IntPtr ptr){
			QHObject result = new QHNull();
			if (ptr == IntPtr.Zero)
				return result;
			
			QHType type = (QHType)qhptr_type (ptr);
			switch (type){
			case QHType.QH_NUMBER:
				result = new QHNumber (number_to_int (ptr));
				break;
			case QHType.QH_STRING:
				{
					// Copy from raw data to buffer
					int length = string_len(ptr);
					byte [] buffer = new byte[length];
					Marshal.Copy (string_to_cstring(ptr), buffer, 0, length);
					result = new QHString (buffer);
				}
				break;
			case QHType.QH_TABLE:
				{
					QHTable table = new QHTable ();
					// Get column name first
					uint cols = table_column_count(ptr);
					table.setTotalColumns (cols);
					uint rows = table_row_count (ptr);

					for (uint c = 0; c < cols ; c++) {
						IntPtr ptrColumnName = table_column_get (ptr, c);
						string column_name = (ptrColumnName != IntPtr.Zero)? Marshal.PtrToStringAuto(ptrColumnName):null;
						if (!string.IsNullOrEmpty(column_name))
							table.setColumnName (c, column_name);
						for (uint r = 0; r < rows; r++) {
							table.setAt (r, c, PtrToQHObject (table_row_get (ptr, r, c)));
						}
					}
					result = table;
				}
				break;
			case QHType.QH_VECTOR:
				{
					QHVector vector = new QHVector ();
					int count = vector_len (ptr);
					for (byte i = 0; i<count; i++)
						vector.setAt(i,PtrToQHObject(vector_at(ptr,i)));
					result = vector;	
				}
				break;
			case QHType.QH_UNDEFINED:
				break;
			}
			return result;
		}

		/// <summary>
		/// Convert QHObject to IntPtr
		/// </summary>
		public static IntPtr QHObjectToIntPtr(QHObject obj){

			IntPtr ptr = IntPtr.Zero;
			if ((obj == null)||(obj is QHNull))
				return ptr;
			
			switch (obj.getType()) {
			case QHType.QH_NUMBER:
				ptr = number_new ((obj as QHNumber).value);
				break;
			case QHType.QH_STRING:
				{
					// Must convert QHString.buffer -> raw pointer
					QHString qhstr = obj as QHString;
					GCHandle guard = GCHandle.Alloc (qhstr.buffer, GCHandleType.Pinned);
					ptr = string_new(qhstr.buffer,qhstr.buffer.Length);
					guard.Free ();
				}
				break;
			case QHType.QH_TABLE:
				{
					QHTable table = (obj as QHTable);
					uint cols = table.getColumnCount ();
					uint rows = table.getRowCount ();
					ptr = table_new (cols);

					// Set name for table
					for (uint c = 0; c < cols; c++) {
						string name = table.columns [c];
						table_column_set (ptr, c, name);
					}

					// Add each records to table
					for (uint r = 0; r < rows; r++) {
						uint row_index = table_row_add (ptr);
						for (uint c = 0; c < cols; c++) {
							QHObject item = table.getAt (row_index, c);
							table_row_set (ptr, row_index, c, QHObjectToIntPtr (item));
						}
					}
				}
				break;
			case QHType.QH_VECTOR:
				{
					QHVector vector = (obj as QHVector);
					ptr = vector_new ();
					int count = vector.Length;
					for (int i = 0; i < count; i++ ) {
						vector_push(ptr,QHObjectToIntPtr(vector.getAt(i)));
					}
				}
				break;
			case QHType.QH_UNDEFINED:
				break;
			}
			return ptr;
		}

	}
		
	public class QHNumber:QHObject{
		public QHNumber(){
			m_Type = QHType.QH_NUMBER;
		}

		public QHNumber(long number){
			m_Type = QHType.QH_NUMBER;
			value = number;
		}

		public long value {
			set;
			get;
		}

		public override string ToString ()
		{
			return value.ToString ();
		}
	}

	public class QHNull:QHObject{
		public override string ToString(){
			return "QHNull";
		}
	}

	public class QHString:QHObject{
		
		public QHString(){
			m_Type = QHType.QH_STRING;
			buffer = new byte[]{ };
		}

		public QHString(string str){
			m_Type = QHType.QH_STRING;
			value = str;
		}

		public QHString (byte[] buffer){
			m_Type = QHType.QH_STRING;
			this.buffer = buffer;
		}

		public string value {
			get {
				return Encoding.UTF8.GetString (buffer);
			}
			set{
				buffer = Encoding.UTF8.GetBytes (value);
			}
		}

		public uint Length{
			get{
				return (uint)buffer.Length;
			}
		}

		public override string ToString (){
			return value;
		}
	
		// Does this prevent changing element of buffer ?
		public byte[] buffer {
			get;
			protected set;
		}
	}

	public class QHVector:QHObject{
		
		public QHVector(){
			m_Type = QHType.QH_VECTOR;
		}

		public void setAt(int index, QHObject obj){
			m_vector [index] = obj;
		}

		public void push(QHObject obj){
			m_vector.Add (m_vector.Count,obj);
		}
			
		public QHObject getAt(int index){
			QHObject obj = null;
			vector.TryGetValue (index, out obj);
			return obj;
		}

		public int Length{
			get {
				return m_vector.Count;
			}
		}

		public Dictionary<int,QHObject> vector{
			get {
				return m_vector;
			}
		}
			
		// Internal vector which is stored
		protected Dictionary<int,QHObject> m_vector = new Dictionary<int,QHObject>();

	}

	public class QHTable:QHObject{
		
		public QHTable(){
			m_Type = QHType.QH_TABLE;
		}
			
		public void setTotalColumns(uint columns)
		{
			for (uint i = 0; i < columns; i++) {
				setColumnName (i, "");
			}
		}

		public void setColumnName(uint col,string name){
			m_columns [col] = name;
		}

		public string getColumnName(uint col){
			string name;
			m_columns.TryGetValue (col, out name);
			return string.IsNullOrEmpty (name) ? "" :name;
		}

		public QHObject getAt(uint row,uint col){

			QHObject obj = null;
			Dictionary<uint,QHObject> record = null;
			if (m_tables.TryGetValue (row, out record)) {
				record.TryGetValue (col, out obj);
			}
			return obj;
		}


		public Dictionary<uint,QHObject> this[uint row]{
			get {
				Dictionary<uint,QHObject> record;
				m_tables.TryGetValue (row, out record);
				return record;
			}
		}
			
		public QHObject this[uint row , uint col]{
			get {
				Dictionary<uint,QHObject> record;
				QHObject result = null;

				if (m_tables.TryGetValue (row, out record) && record.TryGetValue (col, out result))
					return result;
				return result;
			}
			set {
				Dictionary<uint,QHObject> record;
				if (!m_tables.TryGetValue (row, out record))
					m_tables [row] = new Dictionary<uint, QHObject> ();
				m_tables [row] [col] = value;

				// Get the max column
				m_max_column = Math.Max (m_max_column, (int)col);

				// Add missing columns
				for (uint c = 0; c <= (uint)m_max_column; c++) {
					string col_name;
					if (!m_columns.TryGetValue (c, out col_name)) {
						m_columns [c] = "";
					}
				}
			}
		}
			
		public void setAt(uint row,uint col, QHObject obj){
			Dictionary<uint,QHObject> record = null;
			if (!m_tables.TryGetValue (row, out record)) {
				m_tables [row] = new Dictionary<uint,QHObject> ();
			}

			m_tables [row][col] = obj;

			// Get the max column
			m_max_column = Math.Max (m_max_column, (int)col);

			// Add missing columns
			for (uint c = 0; c <= (uint)m_max_column; c++) {
				string col_name;
				if (!m_columns.TryGetValue (c, out col_name)) {
					m_columns [c] = "";
				}
			}
		}

		public uint getColumnCount(){
			return (uint) m_columns.Count;
		}

		public uint getRowCount(){
			return (uint) m_tables.Count;
		}

		public Dictionary<uint,string> columns{
			get {
				return m_columns;
			}
		}

		public Dictionary<uint,Dictionary<uint,QHObject>> table{
			get {
				return m_tables;
			}
		}

		public override string ToString ()
		{
			string content = "";
			for (uint i = 0; i < m_columns.Count; i++) {
				content += string.Format("{0,-15}",m_columns[i]);
			}

			for (uint i = 0; i < m_tables.Count; i++) {
				content += "\n";
				for (uint j = 0; j < m_tables [i].Count; j++) {
					content += string.Format ("{0,-15}", m_tables [i] [j].ToString ());
				}
			}

			return content;
		}

		// Internal vector which is stored
		protected Dictionary<uint,string> m_columns = new Dictionary<uint,string>();
		protected Dictionary<uint,Dictionary<uint,QHObject>> m_tables = new Dictionary<uint,Dictionary<uint,QHObject>>();
		protected int m_max_column = -1;
	}


	public class QHMessage : IDisposable
	{
		#if UNITY_IOS
	    const string dll = "__Internal";
		#elif UNITY_WEBGL && !UNITY_EDITOR
		const string dll = "__Internal";
		#else
		const string dll = "qhmsg";	
		#endif

		/// Message
		[DllImport(dll)]
		extern static IntPtr msg_new(ushort type);
		[DllImport(dll)]
		extern static ushort msg_type(IntPtr msgPtr);

		[DllImport(dll)]
		extern static void msg_delete_all(IntPtr msgPtr);
		[DllImport(dll)]
		extern static int msg_set(IntPtr msgPtr, byte index, IntPtr obj);
		[DllImport(dll)]
		extern static IntPtr msg_get(IntPtr msgPtr, byte index);
		//[DllImport(dll)]
		//extern static uint msg_to_json(IntPtr msgPtr, int opt, [MarshalAs(UnmanagedType.LPArray)] byte []pre_alloc_buf, uint size);

		/// Encode and decode
		[StructLayout(LayoutKind.Sequential)]
		struct msg_decode_result_t{
			public int code;		// MDR_FAILED 0 , MDR_OK 1
			public int consumed;	// Byte need to be cut in buffer event decode ok or failed
			public IntPtr msg;	    // Pointer to native qhmsg_t, must call msg_delete_all() after use
		}

		[DllImport(dll)]
		static extern msg_decode_result_t msg_decode (IntPtr ptr, uint length);

		[DllImport(dll)]
		static extern msg_decode_result_t msg_decode ([MarshalAs(UnmanagedType.LPArray)] byte []buffer, uint length);

		[DllImport(dll)]
		static extern IntPtr msg_encode (IntPtr qhmsg_t);	// This will produce pointer to qhstring_t

		// Delete pointer after encode  
		[DllImport (dll)]
		protected static extern void qhptr_delete (IntPtr qhptr_t);

		// Get length of underlying buffer
		[DllImport(dll)]
		public static extern int string_len(IntPtr ptr);

		// Convert to const char* C string
		[DllImport(dll)]
		protected static extern IntPtr string_to_cstring(IntPtr ptr); 

		// This is wrapping structure for managed code 
		public struct MsgDecodeResult_t{
			public int code;
			public int consumed;
			public QHMessage msg;
		}

		public QHMessage (ushort type){
			m_ptr = msg_new (type);
		}
			
		protected QHMessage(IntPtr msgPtr){
			m_ptr = msgPtr;
		}

		// Only call this for 
		public static QHMessage fromNativePtr(IntPtr msgPtr){
			return new QHMessage (msgPtr);
		}

		// Return Native Pointer
		public IntPtr msgPtr{
			get{
				return m_ptr;
			}
		}
			
		// Implement IDisposable
		public void Dispose()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		bool m_disposed = false;
		IntPtr m_ptr = IntPtr.Zero;

		protected virtual void Dispose(bool disposing)
		{
			if (m_disposed)
				return;

			m_disposed = true;
			if (disposing) {
				// Free all managed resource
			}

			// Free unmanaged resource
			if (m_ptr != IntPtr.Zero) {
				msg_delete_all (m_ptr);
				m_ptr = IntPtr.Zero;
			}
		}

		public int setAt(byte index, QHObject obj)
		{
			IntPtr ptr = QHObject.QHObjectToIntPtr (obj);
			return msg_set (m_ptr, index, ptr);
		}

		public QHObject getAt(byte index){
			IntPtr ptr = msg_get (m_ptr, index);
			return QHObject.PtrToQHObject(ptr);
		}

		public string toJSONString(){
            //byte[] buf = new byte[65535];
            //uint length = msg_to_json (m_ptr, 0, buf, 65535);
            //return Encoding.UTF8.GetString (buf,0,(int)length);
            return "";
		}

        public ushort msgType{
            get{
                return msg_type(m_ptr);
            }
        }
			
		~QHMessage(){
			Dispose (false);
		}

		public byte[] encode(){
			IntPtr ptrNativeBuffer = msg_encode (this.m_ptr);
			int size = string_len(ptrNativeBuffer);
			byte[] result = new byte[size];
			Marshal.Copy (string_to_cstring(ptrNativeBuffer), result, 0, size);

			// Delete qhptr after delete
			qhptr_delete (ptrNativeBuffer);
			return result;
		}

		// Buffer get from socket, do we have to remove consumed byte ?
		public static MsgDecodeResult_t decode(byte[] buffer){
			
			msg_decode_result_t dec = msg_decode (buffer, (uint) buffer.Length);
			MsgDecodeResult_t result = new MsgDecodeResult_t();
			result.code = dec.code;
			result.consumed = dec.consumed;
			if (dec.code == 1 && dec.msg != IntPtr.Zero) {
				result.msg = QHMessage.fromNativePtr (dec.msg);
			} else {
				result.msg = null;
			}
			return result;
		}

		// Buffer get from socket, do we have to remove consumed byte ?
		public static MsgDecodeResult_t decode(IntPtr buffer, int length){

			msg_decode_result_t dec = msg_decode (buffer, (uint) length);
			MsgDecodeResult_t result = new MsgDecodeResult_t();
			result.code = dec.code;
			result.consumed = dec.consumed;
			if (dec.code == 1 && dec.msg != IntPtr.Zero) {
				result.msg = QHMessage.fromNativePtr (dec.msg);
			} else {
				result.msg = null;
			}
			return result;
		}


		// Hexdump 
		public static string hexDump(byte[] bytes, int bytesPerLine = 16)
		{
			if (bytes == null) return "<null>";
			int bytesLength = bytes.Length;

			char[] HexChars = "0123456789ABCDEF".ToCharArray();

			int firstHexColumn =
				8                   // 8 characters for the address
				+ 3;                  // 3 spaces

			int firstCharColumn = firstHexColumn
				+ bytesPerLine * 3       // - 2 digit for the hexadecimal value and 1 space
				+ (bytesPerLine - 1) / 8 // - 1 extra space every 8 characters from the 9th
				+ 2;                  // 2 spaces 

			int lineLength = firstCharColumn
				+ bytesPerLine           // - characters to show the ascii value
				+ Environment.NewLine.Length; // Carriage return and line feed (should normally be 2)

			char[] line = (new String(' ', lineLength - Environment.NewLine.Length) + Environment.NewLine).ToCharArray();
			int expectedLines = (bytesLength + bytesPerLine - 1) / bytesPerLine;
			StringBuilder result = new StringBuilder(expectedLines * lineLength);

			for (int i = 0; i < bytesLength; i += bytesPerLine)
			{
				line[0] = HexChars[(i >> 28) & 0xF];
				line[1] = HexChars[(i >> 24) & 0xF];
				line[2] = HexChars[(i >> 20) & 0xF];
				line[3] = HexChars[(i >> 16) & 0xF];
				line[4] = HexChars[(i >> 12) & 0xF];
				line[5] = HexChars[(i >> 8) & 0xF];
				line[6] = HexChars[(i >> 4) & 0xF];
				line[7] = HexChars[(i >> 0) & 0xF];

				int hexColumn = firstHexColumn;
				int charColumn = firstCharColumn;

				for (int j = 0; j < bytesPerLine; j++)
				{
					if (j > 0 && (j & 7) == 0) hexColumn++;
					if (i + j >= bytesLength)
					{
						line[hexColumn] = ' ';
						line[hexColumn + 1] = ' ';
						line[charColumn] = ' ';
					}
					else
					{
						byte b = bytes[i + j];
						line[hexColumn] = HexChars[(b >> 4) & 0xF];
						line[hexColumn + 1] = HexChars[b & 0xF];
						line[charColumn] = (b < 32 ? '·' : (char)b);
					}
					hexColumn += 3;
					charColumn++;
				}
				result.Append(line);
			}
			return result.ToString();
		}
	}		
}

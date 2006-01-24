//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated by a tool.
//     Runtime Version: 1.1.4322.2032
//
//     Changes to this file may cause incorrect behavior and will be lost if 
//     the code is regenerated.
// </autogenerated>
//------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.ComponentModel;
using System.Collections;
using SharpReportCore;
	
/// <summary>
/// BaseClass for all datahandling Strategies
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 13.11.2005 15:26:02
/// </remarks>

namespace SharpReportCore {	
	public abstract class BaseListStrategy :IDataViewStrategy {
		private bool isSorted = false;
		private bool isFiltered = false;
		private bool isGrouped = false;
		
		//Index to plain Datat
		private SharpArrayList indexList;
		private ReportSettings reportSettings = null;
		private IHierarchicalArray hierarchicalList = null;
		
		
		private ListChangedEventArgs resetList = new ListChangedEventArgs(ListChangedType.Reset,-1,-1);
		
		public event ListChangedEventHandler ListChanged;
		public event EventHandler <GroupChangedEventArgs> GroupChanged;
		
		#region Constructor
		public BaseListStrategy(ReportSettings reportSettings) {
			this.reportSettings = reportSettings;
			this.indexList = new SharpArrayList(typeof(BaseComparer),"IndexList");
		}
		
		#endregion
		
		#region Event's
		protected void FireGroupChange (object source,GroupSeperator groupSeperator) {
			
			if (this.GroupChanged != null) {
				this.GroupChanged (source,new GroupChangedEventArgs(groupSeperator));
			}
		}
			
		
		protected void FireResetList(){
			if (this.ListChanged != null) {
				this.ListChanged (this,this.resetList);
			}
		}
		
		#endregion
		
		protected SharpArrayList IndexList {
			get {
				return indexList;
			}
			set {
				indexList = value;
			}
		}
		
		public ReportSettings ReportSettings {
			get {
				return reportSettings;
			}
		}
		
		
		protected void CheckSortArray (ArrayList arr,string text){
			System.Console.WriteLine("");
			System.Console.WriteLine("{0}",text);
			string tabs = String.Empty;
			
			if (arr != null) {
				int row = 0;
				foreach (BaseComparer bc in arr) {
					GroupSeperator sep = bc as GroupSeperator;
					if (sep != null) {
					
//						System.Console.WriteLine("\t Group change {0} level {1}",sep.ObjectArray[0].ToString(),
//						                         sep.GroupLevel);
						
					} else {
						object [] oarr = bc.ObjectArray;
//						tabs = "\t";
						for (int i = 0;i < oarr.Length ;i++ ) {
							string str = oarr[i].ToString();
							System.Console.WriteLine("\t\t row: {0} {1}",row,str);
						}
						row ++;
					}
					
				}
			}
			System.Console.WriteLine("----------------");
			System.Console.WriteLine("");
		}
		
		
		
		#region SharpReportCore.IDataViewStrategy interface implementation
		
		public virtual ColumnCollection AvailableFields {
			get {
				return new ColumnCollection();
			}
		}
		
		public virtual int Count {
			get {
				return 0;
			}
		}
		
		public virtual int CurrentRow {
			get {
				return 0;
			}
			set {
				if (value > this.indexList.Count){
					throw new IndexOutOfRangeException ("There is no row at " +
					                                    "currentRow: " + value + ".");
				}
				this.indexList.CurrentPosition = value;
			}
		}
		
		public bool HasMoreData {
			get {
				return true;
			}
		}
		
		public virtual bool IsSorted {
			get {
				return this.isSorted;
			}
			set {
				this.isSorted = value;
			}
		}
		
		public bool IsFiltered {
			get {
				return this.isFiltered;
			}
		}
		
		public bool IsGrouped {
			get {
				return this.isGrouped;
			}
			set {
				this.isGrouped = true;
			}
		}
		
		protected virtual void Group() {
			if (this.indexList != null) {
				this.BuildHierarchicalList (this.indexList);
				this.isGrouped = true;
				this.isSorted = true;
			} else {
				throw new NullReferenceException ("BaseListStrategy:Group Sorry, no IndexList");
			}
		}
		
		
		protected IHierarchicalArray HierarchicalList {
			get {
				return hierarchicalList;
			}
		}
		
		
		
		
		
		private GroupSeperator MakeSeperator (BaseComparer newGroup,int groupLevel) {
			
			GroupSeperator seperator = new GroupSeperator (newGroup.ColumnCollection,
			                                               newGroup.ListIndex,
			                                               newGroup.ObjectArray,
			                                               groupLevel);
//			System.Console.WriteLine("");
//			System.Console.WriteLine("\t Group change {0} level {1}",seperator.ObjectArray[0].ToString(),
//			                         seperator.GroupLevel);   
//			System.Console.WriteLine("");
			return seperator;                                              
		}
		
		
		private  IHierarchicalEnumerable BuildHierarchicalList(ArrayList sourceList) {
			IHierarchicalArray destList = new IHierarchicalArray();
			IHierarchicalArray childList = new IHierarchicalArray();
			int level = 0;
			
//			System.Console.WriteLine("");
//			System.Console.WriteLine("BuildHierachicalList");
	
//			ColumnCollection grBy =this.reportSettings.GroupColumnsCollection;
//			string columnName = grBy[level].ColumnName;

			GroupComparer compareComparer = null;
			GroupSeperator seperator = null;
			
			destList.Clear();
			childList.Clear();
			
			for (int i = 0;i < sourceList.Count ;i++ ) {
				GroupComparer currentComparer = (GroupComparer)sourceList[i];
				
				if (compareComparer != null) {
					string str1,str2;
					str1 = currentComparer.ObjectArray[0].ToString();
					str2 = compareComparer.ObjectArray[0].ToString();
					int compareVal = str1.CompareTo(str2);
					
					if (compareVal != 0) {

//						System.Console.WriteLine("child list with {0} entries",childList.Count);
						
						seperator.Childs = childList;
						/*
						//testcode
						if (childList != null) {
							foreach (BaseComparer bc in seperator.Childs) {
								System.Console.WriteLine("\t {0} {1}",bc.ListIndex,
								                         bc.ObjectArray[0].ToString());
							}
						}
						// end testCode
						*/
						
						childList = new IHierarchicalArray();
						seperator = MakeSeperator (currentComparer,level);

						childList.Clear();
						destList.Add (seperator);
					}
				}
				else {
//					System.Console.WriteLine("\t\t Start of List {0}",currentComparer.ObjectArray[0].ToString());
//
//					System.Console.WriteLine("Group change ");
					seperator = MakeSeperator (currentComparer,level);
					childList.Clear();
					destList.Add (seperator);
					seperator.Childs = childList;
				}
//				System.Console.WriteLine("write {0} {1}",currentComparer.ListIndex,currentComparer.ObjectArray[0].ToString());
				
				childList.Add (currentComparer);
				compareComparer = (GroupComparer)sourceList[i];
				
			}
			// Add the last list
			seperator.Childs = childList;
			this.hierarchicalList = destList;
			return destList;
		}
		
		public IHierarchicalEnumerable  IHierarchicalEnumerable {
			get {
				return this.hierarchicalList;
			}
		}
		
		public virtual void Sort() {
			
		}
		
		public  virtual void Reset() {
			this.FireResetList();
		}
		
		public virtual void Bind() {
			
		}
		
		public  virtual void Fill(IItemRenderer item) {
			
		}
		#endregion
		
	}
}

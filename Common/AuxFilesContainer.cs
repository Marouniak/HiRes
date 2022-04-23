using System;
using System.Collections;

namespace HiRes.Common {

	public enum AuxFileType {
		Description = 1,
		Image = 2,
		Design_Scetch = 3
	}

	public class ContentTypes {
		public const string JPEG = "image/jpeg";
		public const string DOC = "text/";
		public const string ARCHIVE = "application/octet-stream";
	}

	public sealed class AuxFilesContainer {
		private Hashtable _auxfiles;

		public AuxFilesContainer() {
			_auxfiles = new Hashtable();
		}
		public AuxFilesContainer(AuxFile[] files) {
			_auxfiles = new Hashtable(files.Length);
			foreach(AuxFile file in files) {
				AddAuxFile(file);
			}
			
		}
		public void AddAuxFile(AuxFile file) {
			if (!_auxfiles.ContainsKey(file.PartId)) {
				_auxfiles.Add(file.PartId,new ArrayList());
			}
			((ArrayList)_auxfiles[file.PartId]).Add(file);
		}

		public void RemoveAuxFileAt(int partId, int pos) {
			if (_auxfiles.ContainsKey(partId)) {
				((ArrayList)_auxfiles[partId]).RemoveAt(pos);
			}
		}

		public Hashtable GetHashtable() {
			return _auxfiles;
		}
	}


	public sealed class AuxFile {
		
		private int auxFileId = PersistentBusinessEntity.ID_EMPTY;
		private int orderId = PersistentBusinessEntity.ID_EMPTY;
		private int partId = PersistentBusinessEntity.ID_EMPTY;
		private string fileName;
		private AuxFileType fileType;
		private String description;
		private string fileContentType;
		private byte[] blob;

		public int AuxFileId {
			get { return auxFileId; }
			set { auxFileId = value; }
		}
		public int OrderId {
			get { return orderId; }
			set { orderId = value; }
		}
		public int PartId {
			get { return partId;}
			set { partId = value;}
		}
		public String FileName {
			get { return fileName; }
			set { fileName = value; }
		}
		public AuxFileType FileType {
			get { return fileType; }
			set { fileType = value; }
		}
		public String Description {
			get { return description; }
			set { description = value; }
		}

		public String FileContentType {
			get { return fileContentType; }
			set { fileContentType = value; }
		}

		public bool IsEmpty {
			get {
				if ((this.FileName==null)||(this.FileName==String.Empty)) {
					return true;
				} else {
					return false;
				}
			}
		}

		public byte[] Blob {
			get { return blob; }
			set { blob = value; }
		}
	}
}

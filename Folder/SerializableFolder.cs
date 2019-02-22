using System;
using System.Collections.Generic;

namespace FolderFile
{
    public struct SerializableFolder : IEquatable<SerializableFolder>
    {
        public string OriginalPath { get; set; }

        public SubfolderType SubType { get; set; }

        public SerializableFolder(string originalPath, SubfolderType subType) : this()
        {
            OriginalPath = originalPath;
            SubType = subType;
        }

        public override bool Equals(object obj)
        {
            return obj is SerializableFolder && Equals((SerializableFolder)obj);
        }

        public bool Equals(SerializableFolder other)
        {
            return OriginalPath == other.OriginalPath &&
                   SubType == other.SubType;
        }

        public override int GetHashCode()
        {
            var hashCode = -269759946;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(OriginalPath);
            hashCode = hashCode * -1521134295 + SubType.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(SerializableFolder folder1, SerializableFolder folder2)
        {
            return folder1.Equals(folder2);
        }

        public static bool operator !=(SerializableFolder folder1, SerializableFolder folder2)
        {
            return !(folder1 == folder2);
        }

        public static implicit operator Folder(SerializableFolder? folder)
        {
            if (folder.HasValue) return new Folder(folder.Value.OriginalPath, folder.Value.SubType);

            return null;
        }

        public static implicit operator SerializableFolder? (Folder folder)
        {
            if (folder != null) return new SerializableFolder(folder.OriginalPath, folder.SubType);

            return null;
        }
    }
}

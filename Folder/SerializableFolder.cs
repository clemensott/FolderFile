using System;
using System.Collections.Generic;

namespace FolderFile
{
    public struct SerializableFolder : IEquatable<SerializableFolder>
    {
        public bool AutoRefresh { get; set; }

        public string OriginalPath { get; set; }

        public SubfolderType SubType { get; set; }

        public SerializableFolder(string originalPath)
            : this(originalPath, Folder.SubTypeDefault, Folder.AutoRefreshDefault)
        {
        }

        public SerializableFolder(string originalPath, SubfolderType subType)
            : this(originalPath, subType, Folder.AutoRefreshDefault)
        {
        }

        public SerializableFolder(string originalPath, bool autoRefresh)
            : this(originalPath, Folder.SubTypeDefault, autoRefresh)
        {
        }

        public SerializableFolder(string originalPath, SubfolderType subType, bool autoRefresh)
        {
            AutoRefresh = autoRefresh;
            OriginalPath = originalPath;
            SubType = subType;
        }

        public bool TryConvert(out Folder folder)
        {
            return Folder.TryCreate(this, out folder);
        }

        public Folder CreateOrDefault()
        {
            return Folder.CreateOrDefault(this);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is SerializableFolder other && Equals(other);
        }

        public bool Equals(SerializableFolder other)
        {
            return AutoRefresh == other.AutoRefresh &&
                   string.Equals(OriginalPath, other.OriginalPath) &&
                   SubType == other.SubType;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = AutoRefresh.GetHashCode();
                hashCode = (hashCode * 397) ^ (OriginalPath != null ? OriginalPath.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)SubType;
                return hashCode;
            }
        }

        public static bool operator ==(SerializableFolder folder1, SerializableFolder folder2)
        {
            return folder1.Equals(folder2);
        }

        public static bool operator !=(SerializableFolder folder1, SerializableFolder folder2)
        {
            return !(folder1 == folder2);
        }

        public static explicit operator Folder(SerializableFolder? folder)
        {
            if (folder.HasValue) return new Folder(folder.Value.OriginalPath, folder.Value.SubType, folder.Value.AutoRefresh);

            return null;
        }

        public static explicit operator SerializableFolder? (Folder folder)
        {
            if (folder != null) return new SerializableFolder(folder.OriginalPath, folder.SubType, folder.AutoRefresh);

            return null;
        }
    }
}

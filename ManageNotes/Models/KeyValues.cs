using System;
using System.Data.Common;
using ManageNotes.Data;

namespace ManageNotes.Models
{
    public static class KeyValues
    {
        public const string OwnerOrAdmin = "0,1";
        public const string Admin = "1";
        public const string Owner = "0";
        public const string Member = "2";
    }
}
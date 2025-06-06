﻿using Storage.Core.Models;

namespace Storage.Core.Interfaces
{
    public interface IStorageRepository
    {
        List<Box> LoadBoxesFromDatabase();
        List<Pallet> LoadPalletsFromDatabase();
    }
}

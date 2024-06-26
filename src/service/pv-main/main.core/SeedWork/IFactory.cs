﻿namespace OpenRIMS.PV.Main.Core.SeedWork
{
	public interface IFactory<TEntity> where TEntity : Entity<long>
	{
		TEntity Create();
	}
}
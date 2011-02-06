﻿using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using StructureMap;
using StructureMap.AutoMocking;

namespace SpecsFor
{
	[TestFixture]
	public abstract class SpecsFor<T> : ITestState<T> where T: class
	{
		protected MoqAutoMocker<T> Mocker;
		protected List<IContext<T>> Contexts = new List<IContext<T>>();

		protected SpecsFor()
		{
			
		}

		protected SpecsFor(Type[] contexts)
		{
			Given(contexts);
		}

		public T SUT { get; set; }

		/// <summary>
		/// Gets the mock for the specified type from the underlying container. 
		/// </summary>
		/// <typeparam name="TMock"></typeparam>
		/// <returns></returns>
		public Mock<TMock> GetMockFor<TMock>() where TMock : class
		{
			return Mock.Get(Mocker.Get<TMock>());
		}

		[SetUp]
		public virtual void SetupEachSpec() 
		{
			Mocker = new MoqAutoMocker<T>();

			ConfigureContainer(Mocker.Container);

			InitializeClassUnderTest();

			Given();

			When();
		}

		protected virtual void InitializeClassUnderTest()
		{
			SUT = Mocker.ClassUnderTest;
		}

		protected virtual void ConfigureContainer(IContainer container)
		{
		}

		[TearDown]
		public virtual void TearDown()
		{
			AfterEachSpec();
		}

		protected virtual void Given()
		{
			Contexts.ForEach(c => c.Initialize(this));
		}

		protected virtual void AfterEachSpec()
		{
			
		}

		protected abstract void When();

		protected void Given<TContext>() where TContext : IContext<T>, new()
		{
			Contexts.Add(new TContext());
		}

		protected void Given(Type[] context)
		{
			var contexts = (from c in context
			                select Activator.CreateInstance(c))
							.Cast<IContext<T>>();

			Contexts.AddRange(contexts);
		}
	}
}
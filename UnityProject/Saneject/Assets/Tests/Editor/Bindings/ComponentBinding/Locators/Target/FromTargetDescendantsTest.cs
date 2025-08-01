﻿using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using Tests.Runtime.Component;
using UnityEngine;
using ComponentRequester = Tests.Runtime.Component.ComponentRequester;

namespace Tests.Editor.Bindings.ComponentBinding.Locators.Target
{
    public class FromTargetDescendantsTest : BaseBindingTest
    {
        private GameObject root, child, grandChild, grandGrandChild;

        [Test]
        public void InjectsConcrete()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = child.AddComponent<ComponentRequester>();
            grandGrandChild.AddComponent<InjectableComponent>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope).FromTargetDescendants();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNotNull(requester.concreteComponent);
        }

        [Test]
        public void InjectsInterface()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = child.AddComponent<ComponentRequester>();
            grandGrandChild.AddComponent<InjectableComponent>();

            // Set up bindings
            BindComponent<IInjectable, InjectableComponent>(scope).FromTargetDescendants();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNotNull(requester.interfaceComponent);
        }

        [Test]
        public void DoesNotInject_WhenNoDescendantMatches()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = child.AddComponent<ComponentRequester>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope).FromTargetDescendants();
            BindComponent<IInjectable, InjectableComponent>(scope).FromTargetDescendants();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNull(requester.concreteComponent);
            Assert.IsNull(requester.interfaceComponent);
        }

        [Test]
        public void InjectsConcrete_IncludeSelfTrue()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            InjectableComponent injectable = child.AddComponent<InjectableComponent>();
            ComponentRequester requester = child.AddComponent<ComponentRequester>();
            TestScope scope = root.AddComponent<TestScope>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope).FromTargetDescendants(includeSelf: true);

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.AreEqual(injectable, requester.concreteComponent);
        }

        [Test]
        public void DoesNotInject_WhenOnlySelfPresentAndIncludeSelfFalse()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            child.AddComponent<InjectableComponent>();
            ComponentRequester requester = child.AddComponent<ComponentRequester>();
            TestScope scope = root.AddComponent<TestScope>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope).FromTargetDescendants(includeSelf: false);

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNull(requester.concreteComponent);
        }

        [Test]
        public void DoesNotInject_WhenNoDescendants()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            GameObject lone = new();
            ComponentRequester requester = lone.AddComponent<ComponentRequester>();
            TestScope scope = lone.AddComponent<TestScope>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope).FromTargetDescendants(includeSelf: false);

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNull(requester.concreteComponent);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject();
            child = new GameObject();
            grandChild = new GameObject();
            grandGrandChild = new GameObject();

            child.transform.SetParent(root.transform);
            grandChild.transform.SetParent(child.transform);
            grandGrandChild.transform.SetParent(grandChild.transform);
        }
    }
}
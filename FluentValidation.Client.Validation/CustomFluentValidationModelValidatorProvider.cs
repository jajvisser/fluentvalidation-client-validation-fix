namespace FluentValidation.Client.Validation
{
    using FluentValidation;
    using FluentValidation.Mvc;
    using System;
    using System.Web.Mvc;

    public class CustomFluentValidationModelValidatorProvider : FluentValidationModelValidatorProvider
    {
	    public Func<Type, bool, ModelMetadata, ControllerContext, Type> CreateValidatorTypeInterceptor { get; set; }

	    public static void Configure(Action<CustomFluentValidationModelValidatorProvider> configurationExpression = null)
	    {
		    configurationExpression = configurationExpression ?? delegate { };

		    var provider = new CustomFluentValidationModelValidatorProvider();
		    configurationExpression(provider);

		    DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
		    ModelValidatorProviders.Providers.Add(provider);
	    }

	    /// <summary>
	    /// This is a workaround to intercept the ModelValidatorProvider.CreateValidator parameters. 
	    /// Now you can add a CreateValidatorTypeInterceptor in the configure method to intercept the CreateValidator and change the model type with custom logic.
	    /// </summary>
	    /// <param name="metadata">container of the current model and property metadata</param>
	    /// <param name="context">controllercontext of the current view</param>
	    /// <returns></returns>
	    protected override IValidator CreateValidator(ModelMetadata metadata, ControllerContext context)
	    {
		    var validatorType = metadata.ModelType;
		    var isProperty = false;

		    if (IsValidatingProperty(metadata))
		    {
			    validatorType = metadata.ContainerType;
			    isProperty = true;
		    }

		    if (CreateValidatorTypeInterceptor != null)
		    {
			    validatorType = CreateValidatorTypeInterceptor(validatorType, isProperty, metadata, context);
		    }

		    return ValidatorFactory.GetValidator(validatorType);
	    }
    }
}
/*
 * Copyright © 2011, Petro Protsyk, Denys Vuika
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *  http://www.apache.org/licenses/LICENSE-2.0
 *  
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace Scripting.SSharp
{
  internal static class Strings
  {
    public const string ArgumentException_EmptyString = @"'{0}' cannot be an empty string ("").";
    public const string InternalExceptionMessage = @"Internal error occurred. Additional information: '{0}'.";
    public const string ArgumentException_Type = @"The type of '{0}' is not supported as an argument.";

    public const string ContextNotFoundExceptionMessage = "Error during invoking event handler: given handler is not associated with any context. This may be a result of Script disposing without unsubscribing from events";
    public const string IdProcessingCanceledByUser = @"Processing identifier '{0}' canceled by event-handler";
    public const string TypeNotFoundError = @"Type with given name '{0}' is not found";
    public const string UsingScopeBindingErrorDuringSetOperation = @"Can't assign value to existing binding '{0}'";
    public const string WrongConfigurationError = "Configuration is wrong or has unsupported format";
    public const string DynamicObjectMethodCallError = "Dynamic method call failed in object {0}";
    public const string MissingOperatorError = "RuntimeHost did not initialize property. Can't find binary operators: '{0}'";
    public const string ForEachMethodNotFound = "GetEnumerator() method missing in object: {0}";
    public const string GrammarErrorExceptionMessage = "Grammar has error";
    public const string NullReferenceInCode = "Null reference in code '{0}'";
    public const string TypeIsNotGeneric = "Error during executing '{0}'. Given type '{1}' is not generic";
    public const string WrongSequenceOfModifiers = "Wrong modifiers sequence in code '{0}'";
    public const string ContextHasCorruptedFlagsError = "Context has its flags corrupted";
    public const string ObjectDoesNotImplementIInvokable = "Error during executing code at '{0}{1}', line:{2} position:{3}, value does not implement IInvokable";
    public const string TypeExpressionIsNotSyntacticallyCorrect = "Type expression is not syntactically correct";
    public const string AssignmentOperatorNotSupported = "Assignment operator: {0} is not supported";
    public const string ScopeParentIsNotValid = "The parent of given scope does not correspond to the root of context";
    public const string GlobalNameNotFound = "Global variable '{0}' not found in scopes";
    public const string MethodNotFound = "Method '{0}' not found";
    public const string MethodSignatureNotFound = "Semantic: There is no method with such signature: {0}({1})";
    public const string NamespaceNotFound = "Namespace '{0}' is not found";
    public const string MemberNotFound = "Member '{0}' not found";
    public const string FunctionNotFound = "Function '{0}' not found";
    public const string VariableNotFound = "Variable '{0}' not found";
    public const string DuplicateEventSubscriptionError = "Duplicate event subscription is not supported";
    public const string LocalIdConflictWithGlobalList = "Cannot create local variable \"{0}\" when it is defined in function global list";

    public const string VerificationNonBoolean = "Condition expression evaluates non boolean value";
    public const string VerificationPreCondition = "Pre condition for function call failed, function '{0}' in code '{1}'";
    public const string VerificationPostCondition = "Post condition for function call failed, function '{0}' in code '{1}'";
    public const string VerificationInvariantCondition = "Invariant for function call failed, function '{0}' in code '{1}'";
  }
}

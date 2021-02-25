using FunctionalExtensions.Base.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionalExtensions.Web
{
    public static class ViewExtensions
    {
        public static IActionResult ToView<TInput>(this TInput target, Controller ctrl, Func<ViewDataDictionary, ViewDataDictionary> setViewData)
        {
            ctrl.ViewData = setViewData(ctrl.ViewData);
            return ctrl.View(target);
        }

        public static IActionResult ToView<TOutput>(this TOutput target, Controller ctrl) =>
            ctrl.View(target);

        public static IActionResult ToView(Controller ctrl) =>
            ctrl.View();

        public static IActionResult ToView(Controller ctrl, Func<ViewDataDictionary, ViewDataDictionary> setViewData)
        {
            ctrl.ViewData = setViewData(ctrl.ViewData);
            return ctrl.View();
        }

        #region RESULT
        public static IActionResult ToView(this Result result, Controller ctrl, Func<ViewDataDictionary, ViewDataDictionary> setViewData)
        {
            ctrl.ViewData["OperationSuccess"] = result.IsSuccess;
            ctrl.ViewData = setViewData(ctrl.ViewData);
            return ctrl.View();
        }

        public static IActionResult ToView(this Result result, Controller ctrl,
                                            Func<ViewDataDictionary, ViewDataDictionary> setViewDataOnSuccess,
                                            Func<ViewDataDictionary, ViewDataDictionary> setViewDataOnFailure)
        {
            ctrl.ViewData =
                result.IsSuccess
                ? ctrl.ViewData = setViewDataOnSuccess(ctrl.ViewData)
                : ctrl.ViewData = setViewDataOnFailure(ctrl.ViewData);
            return ctrl.View();
        }

        public static IActionResult ToResponse(this Result result, Controller ctrl) =>
            result.IsSuccess
            ? ctrl.Ok()
            : ctrl.StatusCode(500);
        #endregion

        #region DATA RESULT
        public static IActionResult ToView<TOutput>(this DataResult<TOutput> dataResult, Controller ctrl, Func<ViewDataDictionary, ViewDataDictionary> setViewData)
        {
            ctrl.ViewData["OperationSuccess"] = dataResult.IsSuccess && dataResult.HasData;

            ctrl.ViewData = setViewData(ctrl.ViewData);

            return
                dataResult.IsSuccess && dataResult.HasData
                ? ctrl.View(dataResult.Data)
                : ctrl.View();
        }

        public static IActionResult ToView<TOutput>(this DataResult<TOutput> dataResult, Controller ctrl)
        {
            ctrl.ViewData["OperationSuccess"] = dataResult.IsSuccess && dataResult.HasData;

            return
                dataResult.IsSuccess && dataResult.HasData
                ? ctrl.View(dataResult.Data)
                : ctrl.View();
        }

        public static IActionResult ToView<TOutput>(this DataResult<TOutput> dataResult, Controller ctrl,
                                                    Func<ViewDataDictionary, ViewDataDictionary> setViewDataOnSuccess,
                                                    Func<ViewDataDictionary, ViewDataDictionary> setViewDataOnFailure)
        {
            ctrl.ViewData =
                dataResult.IsSuccess && dataResult.HasData
                ? ctrl.ViewData = setViewDataOnSuccess(ctrl.ViewData)
                : ctrl.ViewData = setViewDataOnFailure(ctrl.ViewData);

            return
                dataResult.IsSuccess && dataResult.HasData
                ? ctrl.View(dataResult.Data)
                : ctrl.View();
        }

        public static IActionResult ToResponse<TOutput>(this DataResult<TOutput> dataResult, Controller ctrl)
        {
            if (dataResult.IsSuccess)
            {
                if (dataResult.HasData)
                {
                    return ctrl.Ok(dataResult.Data);
                }
                else
                {
                    return ctrl.NoContent();
                }
            }
            else
            {
                return ctrl.StatusCode(500);
            }
        }
        #endregion


        public static IActionResult ToAction<TInput>(this TInput target, Controller ctrl, string actionName) =>
            ctrl.RedirectToAction(actionName);

        public static IActionResult ToAction<TInput>(this TInput target, Controller ctrl, string actionName, Func<ITempDataDictionary, ITempDataDictionary> setTempData)
        {
            ctrl.TempData = setTempData(ctrl.TempData);
            return ctrl.RedirectToAction(actionName);
        }

        public static IActionResult ToErrorCode<TInput>(this TInput target, Controller ctrl, int errorCode) =>
            ctrl.StatusCode(errorCode);

        public static IActionResult ToErrorCode(Controller ctrl, int errorCode) =>
            ctrl.StatusCode(errorCode);

        public static IActionResult ToAction(Controller ctrl, string actionName) =>
            ctrl.RedirectToAction(actionName);

        public static IActionResult ToAction<TInput>(Controller ctrl, string actionName, Func<ITempDataDictionary, ITempDataDictionary> setTempData)
        {
            ctrl.TempData = setTempData(ctrl.TempData);
            return ctrl.RedirectToAction(actionName);
        }
    }

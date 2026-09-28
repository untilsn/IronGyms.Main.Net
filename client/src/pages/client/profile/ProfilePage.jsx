import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import toast from "react-hot-toast";
import { InputText } from "primereact/inputtext";
import { Button } from "primereact/button";
import { Avatar } from "primereact/avatar";
import { Password } from "primereact/password";
import {
  useMyProfile,
  useUpdateProfile,
  useUpdateAvatar,
  useChangePassword,
} from "@/features/profile/hooks/useProfile";
import {
  updateProfileSchema,
  changePasswordSchema,
} from "@/features/profile/schemas/profileSchema";

function ProfilePage() {
  const { data: profile, isLoading } = useMyProfile();
  const updateProfileMutation = useUpdateProfile();
  const updateAvatarMutation = useUpdateAvatar();
  const changePasswordMutation = useChangePassword();

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm({ resolver: zodResolver(updateProfileSchema) });

  useEffect(() => {
    if (profile) {
      reset({
        fullName: profile.fullName,
        phoneNumber: profile.phoneNumber || "",
      });
    }
  }, [profile, reset]);

  const {
    register: registerPassword,
    handleSubmit: handleSubmitPassword,
    reset: resetPasswordForm,
    formState: { errors: passwordErrors },
  } = useForm({ resolver: zodResolver(changePasswordSchema) });

  const onSubmitProfile = (values) => {
    updateProfileMutation.mutate(values, {
      onSuccess: (res) => toast.success(res.message),
      onError: (err) =>
        toast.error(err?.response?.data?.message || "Cập nhật thất bại"),
    });
  };

  const onSubmitPassword = (values) => {
    changePasswordMutation.mutate(values, {
      onSuccess: (res) => {
        toast.success(res.message);
        resetPasswordForm();
      },
      onError: (err) =>
        toast.error(err?.response?.data?.message || "Đổi mật khẩu thất bại"),
    });
  };

  const onAvatarChange = (e) => {
    const file = e.target.files?.[0];
    if (!file) return;
    updateAvatarMutation.mutate(file, {
      onSuccess: (res) => toast.success(res.message),
      onError: (err) =>
        toast.error(err?.response?.data?.message || "Cập nhật ảnh thất bại"),
    });
  };

  if (isLoading) return <p>Đang tải...</p>;

  return (
    <div className="mx-auto max-w-xl space-y-8">
      <section className="rounded-lg border bg-white p-6">
        <div className="mb-4 flex items-center gap-4">
          <Avatar
            image={profile.avatarUrl}
            icon="pi pi-user"
            size="xlarge"
            shape="circle"
          />
          <label className="cursor-pointer text-sm text-blue-600">
            Đổi ảnh đại diện
            <input
              type="file"
              accept="image/*"
              className="hidden"
              onChange={onAvatarChange}
            />
          </label>
        </div>

        <h2 className="mb-4 text-lg font-semibold">Thông tin cá nhân</h2>
        <form
          onSubmit={handleSubmit(onSubmitProfile)}
          className="flex flex-col gap-4"
        >
          <div>
            <label className="mb-1 block text-sm">Họ và tên</label>
            <InputText {...register("fullName")} className="w-full" />
            {errors.fullName && (
              <p className="mt-1 text-sm text-red-600">
                {errors.fullName.message}
              </p>
            )}
          </div>

          <div>
            <label className="mb-1 block text-sm">Số điện thoại</label>
            <InputText {...register("phoneNumber")} className="w-full" />
          </div>

          <Button
            type="submit"
            label="Lưu thay đổi"
            loading={updateProfileMutation.isPending}
          />
        </form>
      </section>

      <section className="rounded-lg border bg-white p-6">
        <h2 className="mb-4 text-lg font-semibold">Đổi mật khẩu</h2>
        <form
          onSubmit={handleSubmitPassword(onSubmitPassword)}
          className="flex flex-col gap-4"
        >
          <div>
            <Password
              {...registerPassword("currentPassword")}
              placeholder="Mật khẩu hiện tại"
              feedback={false}
              toggleMask
              className="w-full"
              inputClassName="w-full"
            />
            {passwordErrors.currentPassword && (
              <p className="mt-1 text-sm text-red-600">
                {passwordErrors.currentPassword.message}
              </p>
            )}
          </div>

          <div>
            <Password
              {...registerPassword("newPassword")}
              placeholder="Mật khẩu mới"
              toggleMask
              className="w-full"
              inputClassName="w-full"
            />
            {passwordErrors.newPassword && (
              <p className="mt-1 text-sm text-red-600">
                {passwordErrors.newPassword.message}
              </p>
            )}
          </div>

          <div>
            <Password
              {...registerPassword("confirmPassword")}
              placeholder="Xác nhận mật khẩu mới"
              feedback={false}
              toggleMask
              className="w-full"
              inputClassName="w-full"
            />
            {passwordErrors.confirmPassword && (
              <p className="mt-1 text-sm text-red-600">
                {passwordErrors.confirmPassword.message}
              </p>
            )}
          </div>

          <Button
            type="submit"
            label="Đổi mật khẩu"
            loading={changePasswordMutation.isPending}
          />
        </form>
      </section>
    </div>
  );
}

export default ProfilePage;

import React, { useContext } from "react";
import { Modal, TransitionablePortal } from "semantic-ui-react";
import { RootStoreContext } from "../../stores/rootStore";
import { observer } from "mobx-react-lite";

const ModalContainer = () => {
  const rootStore = useContext(RootStoreContext);
  const {
    modal: { open, body },
    closeModal,
  } = rootStore.modalStore;

  return (
    <TransitionablePortal open={open} transition={{ animation: "scale", duration: 300 }}>
      <Modal open={open} onClose={closeModal} size="mini">
        <Modal.Content>{body}</Modal.Content>
      </Modal>
    </TransitionablePortal>
  );
};

export default observer(ModalContainer);
